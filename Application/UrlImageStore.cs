using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Security.Cryptography;

// code based on Redth's UrlImageStore, but not using the not very stable NSOperationQueue under MonoTouch but rather 
// the Parallels taks library
// original one: https://gist.github.com/405923

/* usage as follows
 
 	public class ImageManager : IUrlImageUpdated
    {
		public delegate void ImageLoadedDelegate(string id, UIImage image);
		public event ImageLoadedDelegate ImageLoaded;

		UrlImageStore imageStore;
		
        private ImageManager()
        {
			imageStore = new UrlImageStore ("myImageStore", processImage);						            
        }
						
		private static ImageManager instance;
		
		public static ImageManager Instance
		{
			get
			{
				if (instance == null)
					instance = new ImageManager ();
				
				return instance;
			}	
		}
		
		// this is the actual entrypoint you call
		public UIImage GetImage(string imageUrl)
		{
			return imageStore.RequestImage (imageUrl, imageUrl, this);
		}
			
		public void UrlImageUpdated (string id, UIImage image)
		{
			// just propagate to upper level
			if (this.ImageLoaded != null)
				this.ImageLoaded(id, image);
		}
		
		// This handles our ProcessImageDelegate
		// just a simple way for us to be able to do whatever we want to our image
		// before it gets cached, so here's where you want to resize, etc.
		UIImage processImage(string id, UIImage image)
		{
			return image;
		}		
    }
    
    public class MyUIViewController : UIViewController
    {
	    // in some UIViewController simply request the image from the manager and register for the callback delegate to update then
	    // so lets assume we have view controller and it contains imageView instance of UIImageView
	    public override ViewDidLoad ()
	    {
			// get the image by some URL
			UIImage image = ImageManager.Instance.GetProductImage (_imageUrl);	
			
			if (image == null) // it is not available cached, so we will wait for it
			{
				// register for callback
				ImageManager.Instance.ImageLoaded += HandleImageManagerInstanceImageLoaded;
			}
			else
			{
				// it exists so show it here
				if (imageView != null)
					imageView.Image = image;
				
				if (activityIndicator != null)
					activityIndicator.StopAnimating();
			}
	    }
	    
	    void HandleImageManagerInstanceImageLoaded (string id, UIImage image)
		{
			if (id == _imageUrl)
			{
				// deregister the handler
				ImageManager.Instance.ImageLoaded -= HandleImageManagerInstanceImageLoaded;
				
				this.InvokeOnMainThread (delegate {
					
					if (_imageView != null)
						_imageView.Image = image;
					
					if (_activityIndicator != null)
						_activityIndicator.StopAnimating();
				});
			}
		}
	}
	
* */

namespace MonoTouch.UrlImageStore
{
	public interface IUrlImageUpdated
	{
		void UrlImageUpdated(string id, UIImage image);
	}

	public class UrlImageStore : NSObject
	{
		public delegate UIImage ProcessImageDelegate(string id, UIImage img);
		
		readonly static string baseDir;

        readonly string picDir;
        readonly TaskFactory factory;
        readonly LimitedConcurrencyLevelTaskScheduler lcts;
		
		const int CONCURRENT_THREADS = 5; // max number of downloading threads
			
		static UrlImageStore()
		{
			baseDir  = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "..");
		}
		
		public UrlImageStore(string storeName, ProcessImageDelegate processImage)
		{
			StoreName = storeName;
			ProcessImage = processImage;
			
			lcts = new LimitedConcurrencyLevelTaskScheduler (CONCURRENT_THREADS);
            factory = new TaskFactory(lcts);
			
			if (!Directory.Exists(Path.Combine(baseDir, "Library/Caches/Pictures/")))
				Directory.CreateDirectory(Path.Combine(baseDir, "Library/Caches/Pictures/"));
			
			picDir = Path.Combine(baseDir, "Library/Caches/Pictures/" + storeName);			
		}
		
		public void DeleteCachedFiles()
		{
			string[] files = new string[]{};
			
			try { files = Directory.GetFiles(picDir); }
			catch { }
			
			foreach (string file in files)
			{
				try { File.Delete(file); }
				catch { }
			}
		}

		public ProcessImageDelegate ProcessImage
		{
			get;
			private set;
		}
		
		public string StoreName
		{
			get;
			private set;	
		}
		
		/// <summary>
		/// method to generate a MD5 hash of a string
		/// </summary>
		/// <param name="strToHash">string to hash</param>
		/// <returns>hashed string</returns>
		public static string GenerateMD5(string str)
		{
		    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
		
		    byte[] byteArray = Encoding.ASCII.GetBytes(str);
		
		    byteArray = md5.ComputeHash(byteArray);
		
		    string hashedValue = "";
		
		    foreach (byte b in byteArray)
		    {
		        hashedValue += b.ToString("x2");
		    }
		
		    return hashedValue;
		}
		
		public UIImage GetImage (string id)
		{
            string picFile = String.Format("{0}{1}.png", picDir, GenerateMD5(id));
            bool shouldReturn;
            UIImage result = RequestImageImpl(picFile, out shouldReturn);
            if (shouldReturn)
                return result;
			
			return null;
		}

        private static UIImage RequestImageImpl(string picFile, out bool shouldReturn)
        {
            shouldReturn = false;
            if (File.Exists(picFile))
            {
                UIImage img = null;
                try
                {
                    img = UIImage.FromFileUncached(picFile);
                }
                catch
                {
                }
                if (img != null)
                {
                    shouldReturn = true;
                    return img; //Return this image
                }
            }
            return null;
        }

        public UIImage RequestImage(string id, string url, IUrlImageUpdated notify)
		{
			if (string.IsNullOrEmpty (url)) // do not start for empty string, this can happen as cogenta might not have the actual string for the product/offer
				return null;
				
			//Next check for a saved file, and load it into cache and return it if found
            string picFile = String.Format("{0}{1}.png", picDir, GenerateMD5(id));
            bool shouldReturn;
            UIImage result = RequestImageImpl(picFile, out shouldReturn);
            if (shouldReturn)
                return result;

            factory.StartNew(() =>
            {
                try
                {
                    var data = NSData.FromUrl(NSUrl.FromString(url));
                    if (data == null)
                    {
                        Console.WriteLine("UrlImageStore: No data for URL: " + url);
                        return;
                    }
                    var img = UIImage.LoadFromData(data);
                    img = ProcessImage(id, img);
                    AddToCache(id, img);
                    notify.UrlImageUpdated(id, img);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
			
			// Return the default while they wait for the queued download
			return null;
		}

		internal void AddToCache(string id, UIImage img)
		{
            string file = String.Format("{0}{1}.png", picDir, GenerateMD5(id));
			
			if (!File.Exists(file))
			{
				//Save it to disk
				NSError err = null;
				try 
				{ 
					img.AsPNG().Save(file, false, out err); 
					if (err != null)
                        Console.WriteLine(String.Format("{0} - {1}", err.Code, err.LocalizedDescription));
				}
				catch (Exception ex) 
				{
					Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
				}
			}
		}		
	}

	public class UrlImageStoreRequest
	{
		public string Id
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public IUrlImageUpdated Notify
		{
			get;
			set;
		}
	}
	
	/// <summary>
    /// Provides a task scheduler that ensures a maximum concurrency level while
    /// running on top of the ThreadPool.
    /// </summary>
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        /// <summary>Whether the current thread is processing work items.</summary>
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;
        /// <summary>The list of tasks to be executed.</summary>
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)
        /// <summary>The maximum concurrency level allowed by this scheduler.</summary>
        private readonly int _maxDegreeOfParallelism;
        /// <summary>Whether the scheduler is currently processing work items.</summary>
        private int _delegatesQueuedOrRunning; // protected by lock(_tasks)

        /// <summary>
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) 
				throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
			
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }
		
		/// <summary>Queues a task to the scheduler.</summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough
            // delegates currently queued or running to process tasks, schedule another.
            lock (_tasks)
            {
                _tasks.AddLast(task);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary>
        /// Informs the ThreadPool that there's work to be executed for this scheduler.
        /// </summary>
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        base.TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally { _currentThreadIsProcessingItems = false; }
            }, null);
        }

        /// <summary>Attempts to execute the specified task on the current thread.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued"></param>
        /// <returns>Whether the task could be executed on the current thread.</returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems) return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued) TryDequeue(task);

            // Try to run the task.
            return base.TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>Whether the task could be found and removed.</returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks) return _tasks.Remove(task);
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary>
        /// <returns>An enumerable of the tasks currently scheduled.</returns>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken) return _tasks.ToArray();
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    }
}



// 
//  UrlConnection.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
// 
//  Based on Code by Eduardo Scoz. http://escoz.com/nsurlconnection-monotouch-helper-class/
//
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class UrlConnection : NSUrlConnection
	{
		private static Dictionary<string, UrlConnection> Connections = new Dictionary<string, UrlConnection>();

		public static void KillAllConnections()
		{
			foreach (var c in Connections.Values)
			{
				c.Cancel();
			}

			Connections.Clear();
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		protected static void KillConnection(string name)
		{
			Connections[name].Cancel();
			Connections.Remove(name);
		}

		public static void ConnectionEnded(string name)
		{
			Connections.Remove(name);
		}

		public static bool IsDownloading(string name)
		{
			return Connections.ContainsKey(name);
		}

		public UrlConnection(string name, NSUrlRequest request, Action<string> success) : this(name, request, success, null)
		{
		}
		
		public UrlConnection(string name, NSUrlRequest request, Action<string> success, Action failure) : this(name, request, success, failure, null)
		{
		}

		public UrlConnection(string name, NSUrlRequest request, Action<string> success, Action failure, Action<NSUrlConnection, NSUrlAuthenticationChallenge> challenge) : base(request, new UrlDelegate(name, success, failure, challenge), true)
		{
			if (Connections.ContainsKey(name))
			{
				KillConnection(name);
			}

			Connections.Add(name, this);
		}
	}

	public class UrlDelegate : NSUrlConnectionDelegate
	{
		private Action<string> _SuccessAction;
		private Action _FailureAction;
		private Action<NSUrlConnection, NSUrlAuthenticationChallenge> _ChallengeAction;
		private NSMutableData _Data;
		private string _Name;

		public UrlDelegate(string name, Action<string> success): this(name, success, null)
		{
		}

		public UrlDelegate(string name, Action<string> success, Action failure): this(name, success, failure, null)
		{
		}

		public UrlDelegate(string name, Action<string> success, Action failure,  Action<NSUrlConnection, NSUrlAuthenticationChallenge> challenge)
		{
			_Name = name;
			_SuccessAction = success;
			_FailureAction = failure;
			_ChallengeAction = challenge;

			_Data = new NSMutableData();
		}

		public override void ReceivedData(NSUrlConnection connection, NSData d)
		{
			_Data.AppendData(d);
		}

		public override bool CanAuthenticateAgainstProtectionSpace(NSUrlConnection connection, NSUrlProtectionSpace protectionSpace)
		{
			return true;
		}

		public override void ReceivedAuthenticationChallenge(NSUrlConnection connection, NSUrlAuthenticationChallenge challenge)
		{
			if (challenge.PreviousFailureCount > 0)
			{
				challenge.Sender.CancelAuthenticationChallenge(challenge);
				return;
			}
			
			if (challenge.ProtectionSpace.AuthenticationMethod == "NSURLAuthenticationMethodServerTrust")
			{
				challenge.Sender.UseCredentials(NSUrlCredential.FromTrust(challenge.ProtectionSpace.ServerTrust), challenge);
			}
			
			if (_ChallengeAction != null)
				_ChallengeAction(connection, challenge);

//			if (challenge.ProtectionSpace.AuthenticationMethod == "NSURLAuthenticationMethodDefault" && Application.Account != null && Application.Account.Login != null && Application.Account.Password != null)
//			{
//				challenge.Sender.UseCredentials(NSUrlCredential.FromUserPasswordPersistance(Application.Account.Login, Application.Account.Password, NSUrlCredentialPersistence.None), challenge);
//			}
		}

		public override void FailedWithError(NSUrlConnection connection, NSError error)
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			
			if (_FailureAction != null)
				_FailureAction();
		}

		public override void FinishedLoading(NSUrlConnection connection)
		{
			UrlConnection.ConnectionEnded(_Name);

			_SuccessAction(_Data.ToString());
		}
	}
}
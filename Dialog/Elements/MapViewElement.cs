//
// MapElements.cs: Used to render a Google Map
//
// Author:
//  Eduardo Scoz (contact@escoz.com)
//
// Copyright 2010, Eduardo Scoz
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace MonoMobile.Views
{
	using MonoTouch.CoreLocation;
	using MonoMobile.Views;
	using MonoTouch.Foundation;
	using MonoTouch.MapKit;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class MapElement : Element, ISelectable
	{
		public MapElement(string caption, CLLocationCoordinate2D value) : base(caption)
		{
			DataContext = value;
			DataBinding = new MapElementDataBinding(this);
		}

		public override void InitializeCell(UITableView tableView)
		{
			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			TextLabel.Text = Caption;
		
			base.InitializeCell(tableView);
		}

		public void Selected(DialogViewController dvc, UITableView tableView, object item, NSIndexPath path)
		{
			var mapViewController = new MapViewController((CLLocationCoordinate2D)DataContext) { Title = Caption };
			dvc.ActivateController(mapViewController, dvc);
		}

		private class MapViewController : UIViewController
		{
			private MKMapView _MapView;
			private CLLocationCoordinate2D _CurrLocation;

			public MKAnnotation GeocodeAnnotation;

			public MapViewController(CLLocationCoordinate2D newLocation)
			{
				_MapView = CreateMapView();
				_CurrLocation = newLocation;
			}

			public override void ViewDidLoad()
			{
				base.ViewDidLoad();
				View = _MapView;
			}

			public override void ViewWillAppear(bool animated)
			{
				base.ViewWillAppear(animated);
				UpdateLocation(_CurrLocation, false);
			}

			public void UpdateLocation(CLLocationCoordinate2D newLocation, bool animated)
			{
				var span = new MKCoordinateSpan(0.1, 0.1);
				var region = new MKCoordinateRegion(newLocation, span);
				
				_MapView.SetRegion(region, animated);
				
				if (GeocodeAnnotation != null)
					_MapView.RemoveAnnotation(GeocodeAnnotation);
				
				GeocodeAnnotation = new MapViewAnnotation(newLocation);
				
				_MapView.AddAnnotationObject(GeocodeAnnotation);
			}

			private MKMapView CreateMapView()
			{
				var map = new MKMapView { Delegate = new MapViewDelegate(), ZoomEnabled = true, ScrollEnabled = true, ShowsUserLocation = true, MapType = MonoTouch.MapKit.MKMapType.Standard, UserInteractionEnabled = true, MultipleTouchEnabled = true, ClearsContextBeforeDrawing = true, ClipsToBounds = true, AutosizesSubviews = true };
				
				return map;
			}
		}

		private class MapViewDelegate : MKMapViewDelegate
		{
			public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, NSObject annotation)
			{
				var anv = mapView.DequeueReusableAnnotation("thislocation");
				
				if (anv == null)
				{
					var pinanv = new MKPinAnnotationView(annotation, "thislocation");
					pinanv.AnimatesDrop = true;
					pinanv.PinColor = MKPinAnnotationColor.Green;
					pinanv.CanShowCallout = false;
					anv = pinanv;
				} else
				{
					anv.Annotation = annotation;
				}
				return anv;
			}
		}

		private class MapViewAnnotation : MKAnnotation
		{
			public override CLLocationCoordinate2D Coordinate { get;set; }

			public MapViewAnnotation(CLLocationCoordinate2D coord) : base()
			{
				Coordinate = coord;
			}
		}
	}
}

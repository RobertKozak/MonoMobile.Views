//
// MapCellView.cs: Used to render a Google Map
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
// 
// based on code written by
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
	using System.Collections.Generic;
	using System.Drawing;
	using MonoMobile.Views;
	using MonoTouch.CoreLocation;
	using MonoTouch.Foundation;
	using MonoTouch.MapKit;
	using MonoTouch.UIKit;	
	
	[Preserve(AllMembers = true)]
	public class MapCellView : CellView<CLLocationCoordinate2D>, ISelectable, IInitializeCell
	{
		public MapCellView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			var mapAttribute = DataContext.Member.GetCustomAttribute<MapAttribute>();
			if (mapAttribute != null)
			{
				Caption = mapAttribute.Caption;
			}

			cell.TextLabel.Text = Caption;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath path)
		{
			var list = DataContext.Value as IEnumerable<CLLocationCoordinate2D>;
			if (list == null)
			{
				list = new List<CLLocationCoordinate2D>() { (CLLocationCoordinate2D)DataContext.Value };
			}
			var mapViewController = new MapViewController(list) { Title = Caption };
			controller.ActivateController(mapViewController, controller);
		}

		private class MapViewController : UIViewController
		{
			private MKMapView _MapView;
			private IEnumerable<CLLocationCoordinate2D> _Locations;

			public MKAnnotation GeocodeAnnotation;

			public MapViewController(IEnumerable<CLLocationCoordinate2D> newLocations)
			{
				_MapView = CreateMapView();
				_Locations = newLocations;
			}

			public override void ViewDidLoad()
			{
				base.ViewDidLoad();
				View = _MapView;
			}

			public override void ViewWillAppear(bool animated)
			{
				base.ViewWillAppear(animated);
				foreach (var location in _Locations)
				{
					UpdateLocation(location, true);
				}
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

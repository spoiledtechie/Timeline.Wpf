using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {

	/// <summary>
	/// Represent connection between two TimelineItems
	/// </summary>
	public class Connection : Control, INotifyPropertyChanged {

		static Connection() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Connection),
				new FrameworkPropertyMetadata(typeof(Connection)));
		}

		public Connection() : this(null, null) {
		}

		public Connection(Connector source, Connector sink) {
			this.Source = source;
			this.Sink = sink;

			Loaded += new RoutedEventHandler(Connection_Loaded);
			Unloaded += new RoutedEventHandler(Connection_Unloaded);
		}

		private TimelineItem sourceItem;
		public TimelineItem SourceItem {
			get { return sourceItem; }
			set {
				if (sourceItem != value) {
					if (sourceItem != null) {
						sourceItem.PropertyChanged -= OnItemPropertyChanged;
						sourceItem.IsVisibleChanged -= OnItemIsVisibleChanged;
						Source = null;
					}

					sourceItem = value;

					if (sourceItem != null) {
						sourceItem.PropertyChanged += OnItemPropertyChanged;
						sourceItem.IsVisibleChanged += OnItemIsVisibleChanged;
						Source = sourceItem.RightConnector;
					}
				}

				UpdateVisiblity();
			}
		}

		private TimelineItem sinkItem;
		public TimelineItem SinkItem {
			get { return sinkItem; }
			set {
				if (sinkItem != value) {
					if (sinkItem != null) {
						sinkItem.PropertyChanged -= OnItemPropertyChanged;
						sinkItem.IsVisibleChanged -= OnItemIsVisibleChanged;
						Sink = null;
					}

					sinkItem = value;

					if (sinkItem != null) {
						sinkItem.PropertyChanged += OnItemPropertyChanged;
						sinkItem.IsVisibleChanged += OnItemIsVisibleChanged;
						Sink = sinkItem.LeftConnector;
					}
				}

				UpdateVisiblity();
			}
		}

		private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName.Equals("IsDisplayAsZero", StringComparison.InvariantCulture)) {
				UpdateVisiblity();
			}
		}

		private void OnItemIsVisibleChanged(object sender,
			DependencyPropertyChangedEventArgs e) {
			
			UpdateVisiblity();
		}

		private void UpdateVisiblity() {
			if (sourceItem == null || sinkItem == null ||
				sourceItem.Visibility != Visibility.Visible || 
				sinkItem.Visibility != Visibility.Visible) {

				Visibility = Visibility.Collapsed;
			}
			else {
				Visibility = Visibility.Visible;
			}
		}

		// source connector
		private bool sourcePropChngReg;
		private Connector source;
		public Connector Source {
			get {
				return source;
			}
			set {
				if (source != value) {
					if (source != null) {
						UnregisterSource();
						source.Connections.Remove(this);
					}

					source = value;

					if (source != null) {
						source.Connections.Add(this);
						RegisterSource();
					}

					UpdatePathGeometry();
				}
			}
		}

		private void RegisterSource() {
			if (!sourcePropChngReg && source != null) {
				sourcePropChngReg = true;
				source.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
			}
		}

		private void UnregisterSource() {
			if (sourcePropChngReg) {
				sourcePropChngReg = false;
				source.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
			}
		}

		// sink connector
		private bool sinkPropChngReg;
		private Connector sink;
		public Connector Sink {
			get { return sink; }
			set {
				if (sink != value) {
					if (sink != null) {
						UnregisterSink();
						sink.Connections.Remove(this);
					}

					sink = value;

					if (sink != null) {
						sink.Connections.Add(this);
						RegisterSink();
					}
					UpdatePathGeometry();
				}
			}
		}

		private void RegisterSink() {
			if (!sinkPropChngReg && sink != null) {
				sinkPropChngReg = true;
				sink.PropertyChanged += new PropertyChangedEventHandler(OnConnectorPositionChanged);
			}
		}

		private void UnregisterSink() {
			if (sinkPropChngReg) {
				sinkPropChngReg = false;
				sink.PropertyChanged -= new PropertyChangedEventHandler(OnConnectorPositionChanged);
			}
		}

		// connection path geometry
		private PathGeometry pathGeometry;
		public PathGeometry PathGeometry {
			get { return pathGeometry; }
			set {
				if (pathGeometry != value) {
					pathGeometry = value;
					UpdateAnchorPosition();
					OnPropertyChanged("PathGeometry");
				}
			}
		}

		// between source connector position and the beginning 
		// of the path geometry we leave some space for visual reasons; 
		// so the anchor position source really marks the beginning 
		// of the path geometry on the source side
		private Point anchorPositionSource;
		public Point AnchorPositionSource {
			get { return anchorPositionSource; }
			set {
				if (anchorPositionSource != value) {
					anchorPositionSource = value;
					OnPropertyChanged("AnchorPositionSource");
				}
			}
		}

		// slope of the path at the anchor position
		// needed for the rotation angle of the arrow
		private double anchorAngleSource = 0;
		public double AnchorAngleSource {
			get { return anchorAngleSource; }
			set {
				if (anchorAngleSource != value) {
					anchorAngleSource = value;
					OnPropertyChanged("AnchorAngleSource");
				}
			}
		}

		// analogue to source side
		private Point anchorPositionSink;
		public Point AnchorPositionSink {
			get { return anchorPositionSink; }
			set {
				if (anchorPositionSink != value) {
					anchorPositionSink = value;
					OnPropertyChanged("AnchorPositionSink");
				}
			}
		}
		// analogue to source side
		private double anchorAngleSink = 0;
		public double AnchorAngleSink {
			get { return anchorAngleSink; }
			set {
				if (anchorAngleSink != value) {
					anchorAngleSink = value;
					OnPropertyChanged("AnchorAngleSink");
				}
			}
		}

		private ArrowSymbol sourceArrowSymbol = ArrowSymbol.None;
		public ArrowSymbol SourceArrowSymbol {
			get { return sourceArrowSymbol; }
			set {
				if (sourceArrowSymbol != value) {
					sourceArrowSymbol = value;
					OnPropertyChanged("SourceArrowSymbol");
				}
			}
		}

		public ArrowSymbol sinkArrowSymbol = ArrowSymbol.Arrow;
		public ArrowSymbol SinkArrowSymbol {
			get { return sinkArrowSymbol; }
			set {
				if (sinkArrowSymbol != value) {
					sinkArrowSymbol = value;
					OnPropertyChanged("SinkArrowSymbol");
				}
			}
		}

		// specifies a point at half path length
		private Point labelPosition;
		public Point LabelPosition {
			get { return labelPosition; }
			set {
				if (labelPosition != value) {
					labelPosition = value;
					OnPropertyChanged("LabelPosition");
				}
			}
		}

		// pattern of dashes and gaps that is used to outline the connection path
		private DoubleCollection strokeDashArray;
		public DoubleCollection StrokeDashArray {
			get {
				return strokeDashArray;
			}
			set {
				if (strokeDashArray != value) {
					strokeDashArray = value;
					OnPropertyChanged("StrokeDashArray");
				}
			}
		}

		void OnConnectorPositionChanged(object sender, PropertyChangedEventArgs e) {
			// whenever the 'Position' property of the source or sink Connector 
			// changes we must update the connection path geometry
			if (e.PropertyName.Equals("Position")) {
				UpdatePathGeometry();
			}
		}

		protected override Size ArrangeOverride(Size arrangeBounds) {
			return base.ArrangeOverride(arrangeBounds);
		}

		private void UpdatePathGeometry() {
			if (Source != null && Sink != null) {
				PathGeometry geometry = new PathGeometry();
				PathFigure figure = new PathFigure();
				figure.StartPoint = new Point(source.Position.X + 5, source.Position.Y);
				figure.Segments.Add(new LineSegment(new Point(source.Position.X + 10, source.Position.Y), true));
				figure.Segments.Add(new LineSegment(new Point(sink.Position.X - 10, sink.Position.Y), true));
				figure.Segments.Add(new LineSegment(new Point(sink.Position.X - 5, sink.Position.Y), true));
				geometry.Figures.Add(figure);
				this.PathGeometry = geometry;
				//List<Point> linePoints = PathFinder.GetConnectionLine(Source.GetInfo(), Sink.GetInfo(), true);
				//if (linePoints.Count > 0) {
				//  PathFigure figure = new PathFigure();
				//  figure.StartPoint = linePoints[0];
				//  linePoints.Remove(linePoints[0]);
				//  figure.Segments.Add(new PolyLineSegment(linePoints, true));
				//  geometry.Figures.Add(figure);

				//  this.PathGeometry = geometry;
				//}
			}
		}

		private void UpdateAnchorPosition() {
			Point pathStartPoint, pathTangentAtStartPoint;
			Point pathEndPoint, pathTangentAtEndPoint;
			Point pathMidPoint, pathTangentAtMidPoint;

			// the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector 
			// on PathGeometry at the specified fraction of its length
			this.pathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
			this.pathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);
			this.pathGeometry.GetPointAtFractionLength(0.5, out pathMidPoint, out pathTangentAtMidPoint);

			// get angle from tangent vector
			//this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X) * (180 / Math.PI);
			//this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);
			this.AnchorAngleSource = 180;
			this.AnchorAngleSink = 0;

			// add some margin on source and sink side for visual reasons only
			pathStartPoint.Offset(-pathTangentAtStartPoint.X * 5, -pathTangentAtStartPoint.Y * 5);
			pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

			this.AnchorPositionSource = pathStartPoint;
			this.AnchorPositionSink = pathEndPoint;
			this.LabelPosition = pathMidPoint;
		}

		private void Connection_Loaded(object sender, RoutedEventArgs e) {
			RegisterSource();
			RegisterSink();
		}

		void Connection_Unloaded(object sender, RoutedEventArgs e) {
			// do some housekeeping when Connection is unloaded

			// remove event handler
			UnregisterSource();
			UnregisterSink();
		}

		// we could use DependencyProperties as well to inform others of property changes
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(name));
			}
		}



		public Brush LineStroke {
			get { return (Brush)GetValue(LineStrokeProperty); }
			set { SetValue(LineStrokeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LineStrokeProperty =
				DependencyProperty.Register("LineStroke", typeof(Brush), typeof(Connection), new UIPropertyMetadata(Brushes.Black));




		public double LineStrokeThickness {
			get { return (double)GetValue(LineStrokeThicknessProperty); }
			set { SetValue(LineStrokeThicknessProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LineStrokeThickness.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LineStrokeThicknessProperty =
				DependencyProperty.Register("LineStrokeThickness", typeof(double), typeof(Connection), new FrameworkPropertyMetadata(2D));

	}

	public enum ArrowSymbol {
		None,
		Arrow,
		Diamond
	}

}

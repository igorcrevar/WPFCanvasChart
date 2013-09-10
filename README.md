# WPFCanvasChart

### Customizable WPF charting for large number of elements
There are controls for: Line series charts, Bar charts, Stacked bar charts, etc. 
You can also use WPFCanvasChartComponent class directly(look at WPFCanvasChartTest example). The class will do for you things like axis drawing, point translation, zooming, scrolling, etc... Actual chart content is drawn by user.
Both WPFCanvasChartComponent and WPFChartControl are fast and customizable.

### How to use?
Its easy! Just look at WPFChartControlExample or WPFCanvasChartTest example projects in solution.
Using chart control is easy. In XAML:
``` xml
...
xmlns:chart="clr-namespace:IgorCrevar.WPFChartControl;assembly=WPFChartControl"
...
<chart:ChartControl Drawer="{Binding Path=YourCustomDrawer}" />
```
In ViewModel you can do something like:
``` c#
YourCustomDrawer = new LineSeriesChartDrawer(new List<Point> { new Point(10, 20), new Point(20, 40) });
```
or
``` c#
YourCustomDrawer = new StackedBarChartDrawer(new List<StackedBarItem>
{
   new StackedBarItem(0, new double[] { 50, 30, 20 }),
   new StackedBarItem(1, new double[] { 100, 0, 100 }),
})
{
	Legend = new LegendItem[]
	{
		new LegendItem(Colors.Blue, "One"),
		new LegendItem(Colors.Red, "Two"),
		new LegendItem(Colors.Yellow, "Total"),
	},
	LegendWidth = 120.0d,
};
```
or
``` c#
BarChartDrawer = new BarChartDrawer(new Point[]{
	new Point(1.0d, rnd.Next(100)),
	new Point(2.0d, rnd.Next(100)),
	new Point(3.0d, rnd.Next(100)),
	new Point(4.0d, rnd.Next(100)),
});
```
etc
Check example for more options for every drawer...

Also you can use WPFCanvasChartComponent directly (from WPFCanvasChart project)
- Before using, instance of WPFCanvasChart must be created
- after that, you must set min/max x/y of your chart values using WPFCanvasChart  instance SetMinMax method
- you must implement IWPFCanvasChartDrawer interface
- draw call on WPFCanvasChart instance will draw chart(step above must be executed before drawing!)
- IWPFCanvasChartDrawer method public void Draw(DrawingContext ctx) is used to draw actual graph
- Inside public void Draw(DrawingContext ctx) use Point2ChartPoint to convert your actual value to chart point
- Simple bar chart control is included as separate project(WPFBarChartControl). It provides easy and 
customizable bar chart creation with legend. Look at WPFBarChartExample project for detailed explanation

#### Note:
Chart is fast if canvas used for charting is inside Viewbox(like in code snippet bellow). If canvas is inside some other container(for example grid like in second code snippet bellow. This is how chart is created inside chart control), than performances dramatically decrease. Why? I do not know yet.
``` xml
<!-- fast -->
<Viewbox Stretch="Fill" Grid.Row="1">
	<Canvas Name="Canvas" Width="350" Height="190" />
</Viewbox>

<!-- slow :( -->
<Grid Grid.Row="0" Grid.Column="1" Name="CanvasParent" 
	HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
	<Canvas Name="Canvas" Width="{Binding Path=ActualWidth, ElementName=CanvasParent, Mode=OneWay}"
		Height="{Binding Path=ActualHeight, ElementName=CanvasParent, Mode=OneWay}"/>
</Grid>
```

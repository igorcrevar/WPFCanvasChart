# WPFCanvasChart

### WPF line graph charting for large number of elements
(although bar chart is also possible, but project is not ment for that)

#### Description
Flexibile and easy to use charting for wpf. It rel·a·tive·ly fast, for example, on my computer Amilo 3560, 8000 points can be draw and zoom/scroll very fast.
12000+ points are slow :(. Much faster than chart from WPFToolkit, but does not allow binding(drawing must be done by programmer itself), although one can
extend base class and allow binding

#### How to use?
Its easy! Just look at test project. Also:
- Before using, instance of WPFCanvasChart must be created
- after that, you must set min/max x/y of your chart values using WPFCanvasChart  instance SetMinMax method
- you must implement IWPFCanvasChartDrawer interface
- draw call on WPFCanvasChart instance will draw chart(step above must be executed before drawing!)
- IWPFCanvasChartDrawer method public void Draw(DrawingContext ctx) is used to draw actual graph
- Inside public void Draw(DrawingContext ctx) use Point2ChartPoint to convert your actual value to chart point


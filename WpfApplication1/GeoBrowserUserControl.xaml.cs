using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GeoBrowserUserControl : UserControl
    {
        private static int foo = 0;
        private static bool doneOnce = false;
        private ChromiumWebBrowser Browser { get; set; }

        public GeoBrowserUserControl()
        {
            InitializeComponent();
            // Cef.Initialize(new CefSettings());

            CefSharp.CefSettings settings = new CefSharp.CefSettings();
            settings.PackLoadingDisabled = true;

            Browser = new CefSharp.Wpf.ChromiumWebBrowser();
            this.mainDock.Children.Add(Browser);
            Browser.Address = "http://192.168.159.128:8000/Apps/HelloWorld.html";

            //Wait for the page to finish loading (all resources will have been loaded, rendering is likely still happening)
            Browser.MouseDoubleClick += (sender, args) =>
            {
                Browser.ExecuteScriptAsync("addCircle", foo, foo, "1000000");
                foo += 10;
            };

            Browser.LoadingStateChanged += (sender, args) =>
            {
                //Wait for the Page to finish loading
                if (args.IsLoading == false)
                {

                }
            };
            //Wait for the MainFrame to finish loading
            Browser.FrameLoadEnd += (sender, args) =>
            {
                //Wait for the MainFrame to finish loading
                if (args.Frame.IsMain)
                {
                    if (!doneOnce)
                    {
                        doneOnce = true;
                        Browser.ExecuteScriptAsync(SetUpScript);
                    }
                }
            };

        }



        const string SetUpScript = @"


                            var scene = viewer.scene;
                            var primitives = scene.primitives;
                            var ellipsoid = scene.globe.ellipsoid;

                            var addCircle = function (x,y,r) {
                                // Red circle
                                var circleGeometry = new Cesium.CircleGeometry({
                                    center : ellipsoid.cartographicToCartesian(Cesium.Cartographic.fromDegrees(x, y)),
                                    radius: r,
                                    stRotation: Cesium.Math.toRadians(90.0),
                                    vertexFormat: Cesium.PerInstanceColorAppearance.VERTEX_FORMAT
                                });
                                var redCircleInstance = new Cesium.GeometryInstance({
                                    geometry : circleGeometry,
                                    attributes:
                                                    {
                                                    color: Cesium.ColorGeometryInstanceAttribute.fromColor(new Cesium.Color(1.0, 0.0, 0.0, 0.5))
                                    }
                                                });



                                // Add instances to primitives 
                                primitives.add(new Cesium.Primitive({
                                    geometryInstances: redCircleInstance,
                                    appearance: new Cesium.PerInstanceColorAppearance({
                                        closed: true
                                    })
                                }));

                           
  
                            };
                            addCircle(1,1,1000000);
                            viewer.camera.setView({
                                destination: Cesium.Rectangle.fromDegrees(114.591, -45.837, 148.970, -5.730)
                            });
                            ";


    }
}

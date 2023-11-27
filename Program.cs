using System;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;



namespace Raytrace
{
    class Program : RaytracingLib
    {


        static int imagewidth = 1920;
        static int imageheight = 1080;



        static Ray Camera = new Ray(new float3(0, 2, -3), new float3(0, 0, 1));
        static Ray[][][] pixelrays;
        static Triangle[] Scenetris = new Triangle[5];
        static public void Main()
        {
            float frameTime = 0;
            Image<Rgba32> image = new Image<Rgba32>(imagewidth, imageheight);
            Image<Rgba32> skybox = Image.Load<Rgba32>(@"industrial_sunset_puresky_2k.png");
            DateTime timeStampStart = DateTime.UtcNow;
            Scenetris[0] = new Triangle(new float3(2, 0, 2), new float3(-2, 0, 2), new float3(0, 4, 2f));
            Scenetris[1] = new Triangle(new float3(6, 0, 2), new float3(2, 0, 2), new float3(4, 2, 2.5f));
            Scenetris[2] = new Triangle(new float3(-6, 0, 2), new float3(-2, 0, 4), new float3(-4, 4, 1.9f));
            Scenetris[3] = new Triangle(new float3(-100, 0, 100), new float3(100, 0, 100), new float3(100, 0, -100));
            Scenetris[4] = new Triangle(new float3(-100, 0, -100), new float3(-100, 0, 100), new float3(100, 0, -100));

            //initialize the entire pixelray array.
            pixelrays = MakeCameraRayArray(imagewidth, imageheight, Camera, 0.4f);

            for (int i = 0; i < pixelrays.Length; i++)
            {
                Parallel.ForEach(pixelrays[i], (Ray, state, index) =>
                {
                    for (int i = 0; i < Ray.Length; i++)
                    {
                        float3 intersect;
                        bool isColored = false;
                        foreach (Triangle tri in Scenetris)
                        {

                            intersect = CalculateIntersection(tri, Ray[0]);

                            if (intersect.x == 0 && intersect.y == 0 && intersect.z == 0 && !isColored)
                            {
                                float3 test = MapRayToSkybox(Ray[0]);
                                image[i, (int)index] = skybox[(int)(skybox.Width * test.y + skybox.Width * 0.5), (int)((skybox.Height * test.x) + (skybox.Height * 0.5))];
                            }
                            else
                            {

                                if (!isColored)
                                {
                                    isColored = true;
                                    image[i, (int)index] = Color.FromRgba(Convert.ToByte(255 * ((tri.NormalPlane.x / 2) + 0.5)), Convert.ToByte(255 * ((tri.NormalPlane.y / 2) + 0.5)), Convert.ToByte(255 * ((tri.NormalPlane.z / 2) + 0.5)), 255);
                                }


                            }
                        }
                    }



                });





            }
            System.Console.WriteLine("{0}   {1}", skybox.Width, skybox.Height);
            frameTime = (float)(DateTime.UtcNow - timeStampStart).TotalMilliseconds;
            System.Console.WriteLine(frameTime);
            Console.WriteLine("hey");
            image.Save(@"test.png");
            frameTime = (float)(DateTime.UtcNow - timeStampStart).TotalMilliseconds;
            System.Console.WriteLine(frameTime);

        }






    }
}

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


        static int imagewidth = 32;
        static int imageheight = 32;



        static Ray Camera = new Ray(new float3(0, 2, 0), new float3(0, 0, 1));
        static Ray[][] pixelrays;
        static Triangle[] Scenetris = new Triangle[3];
        static public void Main()
        {
            float frameTime = 0;
            Image<Rgba32> image = new Image<Rgba32>(imagewidth, imageheight);
            DateTime timeStampStart = DateTime.UtcNow;
            Scenetris[0] = new Triangle(new float3(2, 0, 2), new float3(-2, 0, 2), new float3(0, 4, 2f));
            Scenetris[1] = new Triangle(new float3(6, 0, 2), new float3(2, 0, 2), new float3(4, 2, 2.5f));
            Scenetris[2] = new Triangle(new float3(-6, 0, 2), new float3(-2, 0, 4), new float3(-4, 4, 1.9f));


            //initialize the entire pixelray array.
            pixelrays = MakeCameraRayArray(imagewidth, imageheight, Camera, 0.15f);
            
            for (int i = 0; i < pixelrays.Length; i++)
            {
                Parallel.ForEach(pixelrays[i], (Ray, state, index) =>
                {
                    bool isColored = false;
                    foreach (Triangle tri in Scenetris)
                    {
                        float3 intersect = CalculateIntersection(tri, Ray);
                        
                        if (intersect.x == 0 && intersect.y == 0 && intersect.z == 0)
                        {
                            System.Console.WriteLine("false");
                            image[i, (int)index] = Color.White;

                        }
                        else
                        {
                            
                            if (!isColored)
                            {
                                isColored = true;
                                System.Console.WriteLine("{0}hey   {1},{2},{3}",index, Convert.ToByte(255 * ((tri.NormalPlane.x / 2) + 0.5)), Convert.ToByte(255 * ((tri.NormalPlane.y / 2) + 0.5)), Convert.ToByte(255 * ((tri.NormalPlane.z / 2) + 0.5)));
                                image[i, (int)index] = Color.FromRgba(Convert.ToByte(255 * ((tri.NormalPlane.x / 2) + 0.5)), Convert.ToByte(255 * ((tri.NormalPlane.y / 2) + 0.5)), Convert.ToByte(255 * ((tri.NormalPlane.z / 2) + 0.5)), 255);
                            }
                            else{System.Console.WriteLine("is colored");}
                            
                        }
                    }

                });





            }

            frameTime = (float)(DateTime.UtcNow - timeStampStart).TotalMilliseconds;
            System.Console.WriteLine(frameTime);
            Console.WriteLine("hey");
            image.Save(@"test.png");
            frameTime = (float)(DateTime.UtcNow - timeStampStart).TotalMilliseconds;
            System.Console.WriteLine(frameTime);

        }






    }
}

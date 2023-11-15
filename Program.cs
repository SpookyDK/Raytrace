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
        static float kage;
        static float pixeldistance = 0.003f;
        static int imagewidth = 128;
        static int imageheight = 128;



        static Ray Camera = new Ray(new float3(1, 2, -5), new float3(0, 0, 1));
        static Ray[][] pixelrays;

        static public void Main()
        {
            float frameTime = 0;
            Image<Rgba32> image = new Image<Rgba32>(imagewidth, imageheight);
            DateTime timeStampStart = DateTime.UtcNow;
            Triangle tri = new Triangle(new float3(2, 0, 2), new float3(-2, 0, 2), new float3(0, 4, 2));



            //initialize the entire pixelray array.
            pixelrays = MakeCameraRayArray(imagewidth, imageheight, Camera, 0.05f);
            
            for (int i = 0; i < pixelrays.Length; i++)
            {
                Parallel.ForEach(pixelrays[i], (Ray, state, index) =>
                {
                    float3 intersect = CalculateIntersection(tri, Ray);
                    if (intersect.x == 0 && intersect.y == 0 && intersect.z == 0)
                    {
                        image[i, (int)index] = Color.Blue;
                    }
                    else { image[i, (int)index] = Color.White; }
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

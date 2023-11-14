using System;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;



namespace RaytracingBasics
{
    class Program : RaytracingLib
    {
        static float pixeldistance = 0.003f;
        static int imagewidth = 1920;
        static int imageheight = 1080;



        static Ray Camera = new Ray(new float3(-6, 6, -2), new float3(0, 0, 1));
        static Ray[][] pixelrays;

        static public void Main()
        {
            float frameTime = 0;
            Image<Rgba32> image = new Image<Rgba32>(imagewidth, imageheight);
            DateTime timeStampStart = DateTime.UtcNow;
            Triangle tri = new Triangle(new float3(2, 0, 2), new float3(-2, 0, 2), new float3(0, 4, 2));



            //initialize the entire pixelray array.
            pixelrays = new Ray[imagewidth][];
            for (int i = 0; i < pixelrays.Length; i++)
            {
                pixelrays[i] = new Ray[imageheight];
                for (int j = 0; j < pixelrays[i].Length; j++)
                {
                    pixelrays[i][j].origin = new float3(Camera.origin.x + i * pixeldistance, Camera.origin.y - j * pixeldistance, Camera.origin.z + i * pixeldistance);
                    pixelrays[i][j].direction = Camera.direction;
                }
            }
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

                /*
                }
                for (int j = 0; j < pixelrays[i].Length; j++)
                {
                    if (CalculateIntersection(tri, pixelrays[i][j]).x == 0 && CalculateIntersection(tri, pixelrays[i][j]).y == 0 && CalculateIntersection(tri, pixelrays[i][j]).z == 0)
                    {
                        image[i, j] = Color.Blue;
                    }
                    else { image[i, j] = Color.White; }
                }*/




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

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


        static int imagewidth = 128;
        static int imageheight = 128;



        static Ray Camera = new Ray(new float3(0, 2, -3), new float3(0, 0, 1));
        static Ray[][][] pixelrays;
        static Triangle[] Scenetris = new Triangle[5];
        static public void Main()
        {
            float frameTime = 0;
            Image<Rgba32> image = new Image<Rgba32>(imagewidth, imageheight);
            Image<Rgba32> skybox = Image.Load<Rgba32>(@"industrial_sunset_puresky_2k.png");
            DateTime timeStampStart = DateTime.UtcNow;
            Scenetris[0] = new Triangle(new float3(2, 0, 2), new float3(-2, 0, 2), new float3(0, 4, 2f), new float3(0.9f, 0.9f, 0.9f));
            Scenetris[1] = new Triangle(new float3(6, 0, 2), new float3(2, 0, 2), new float3(4, 2, 2.5f), new float3(0.9f, 0.9f, 0.9f));
            Scenetris[2] = new Triangle(new float3(-6, 0, 2), new float3(-2, 0, 4), new float3(-4, 4, 1.9f), new float3(0.9f, 0.9f, 0.9f));
            Scenetris[3] = new Triangle(new float3(-100, 0, 100), new float3(100, 0, 100), new float3(100, 0, -100), new float3(0.9f, 0.9f, 0.9f));
            Scenetris[4] = new Triangle(new float3(-100, 0, -100), new float3(-100, 0, 100), new float3(100, 0, -100), new float3(0.9f, 0.9f, 0.9f));




            pixelrays = MakeCameraRayArray(imagewidth, imageheight, Camera, 0.4f);

            for (int i = 0; i < pixelrays.Length; i++)
            {
                Parallel.ForEach(pixelrays[i], (Ray, state, index) =>
                {
                    for (int i = 0; i < Ray.Length; i++)
                    {
                        float4 intersect = new float4(0, 0, 0, 100);
                        Triangle temptri = new Triangle(new float3(0, 0, 0), new float3(0, 0, 0), new float3(0, 0, 0), new float3(0, 0, 0));


                        foreach (Triangle tri in Scenetris)
                        {

                            float4 temp = CalculateIntersection(tri, Ray[0]);
                            if (temp.q < intersect.q)
                            {
                                intersect = temp;
                                temptri = tri;
                            }


                        }
                        if (intersect.x == 0 && intersect.y == 0 && intersect.z == 0)
                        {
                            Ray[10].origin = new float3(0, 0, 0);
                            Ray[10].Strength = Ray[i].Strength;
                            Ray[10].direction = Ray[i].direction;
                            break;
                        }
                        else
                        {
                            if (i < Ray.Length - 1)
                            {
                                Ray[i + 1].Strength = new float3(Ray[i].Strength.x * temptri.ColorAbsorb.x, Ray[i].Strength.y * temptri.ColorAbsorb.y, Ray[i].Strength.z * temptri.ColorAbsorb.z);
                                Ray[i + 1].origin = new float3(intersect.x, intersect.y, intersect.z);
                                Ray[i + 1].direction = ReflektRay(Ray[i], temptri);
                            }
                        }
                        
                    }
                    float3 test = MapRayToSkybox(Ray[10]);
                    image[i, (int)index] = skybox[(int)(skybox.Width * test.y + skybox.Width * 0.5), (int)((skybox.Height * test.x) + (skybox.Height * 0.5))];



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

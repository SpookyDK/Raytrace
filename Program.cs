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
            Scenetris[0] = new Triangle(new float3(2, 0, 2), new float3(-2, 0, 2), new float3(0, 4, 3f), new float3(0.9f, 0.5f, 0.5f));
            Scenetris[1] = new Triangle(new float3(6, 0, 2), new float3(2, 0, 2), new float3(4, 2, 2.5f), new float3(0.5f, 0.9f, 0.5f));
            Scenetris[2] = new Triangle(new float3(-6, 2, 2), new float3(-2, 2, 4), new float3(-4, 6, 1f), new float3(1f,1f,1f));
            Scenetris[3] = new Triangle(new float3(-5, -2, 5), new float3(5, -2, 5), new float3(5, -2, -5), new float3(0.3f,0.3f,0.3f));
            Scenetris[4] = new Triangle(new float3(-5, -2, -5), new float3(-5, -2, 5), new float3(5, -2, -5), new float3(0.3f,0.3f,0.3f));
            



            pixelrays = MakeCameraRayArray(imagewidth, imageheight, Camera, 0.4f);

            for (int i = 0; i < pixelrays.Length; i++)
            {
                Parallel.ForEach(pixelrays[i], (Ray, state, index) =>
                {
                    Ray = TraceRays(Ray, Scenetris);
                    int templength = Ray.Length-1;
                    //System.Console.WriteLine("ray {0}    {1}    {2}",Ray[0].direction.x,Ray[0].direction.y,Ray[0].direction.z);
                    //System.Console.WriteLine("ray {0}    {1}    {2}", Ray[10].direction.x,Ray[10].direction.y,Ray[10].direction.z);
                    float3 test = MapRayToSkybox(Ray[templength]);
                    int temp1 = (int)(skybox.Width * test.y + skybox.Width * 0.5);
                    int temp2 = (int)((skybox.Height * test.x) + (skybox.Height * 0.5));
                    
                    
                    image[i, (int)index] = Color.FromRgb(Convert.ToByte((int)(skybox[temp1, temp2].R* Ray[templength].Strength.x)), Convert.ToByte((int)(skybox[temp1, temp2].G * Ray[templength].Strength.y)), Convert.ToByte((int)(skybox[temp1, temp2].B * Ray[templength].Strength.z)));
                    


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

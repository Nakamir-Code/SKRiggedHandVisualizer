using StereoKit;
using System;

namespace RiggedHandVisualizer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Initialize StereoKit
            SKSettings settings = new SKSettings
            {
                appName = "RiggedHandVisualizer",
                assetsFolder = "Assets",
            };
            if (!SK.Initialize(settings))
                Environment.Exit(1);


            // Create assets used by the app

            Model cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);

            Matrix floorTransform = Matrix.TS(0, -1.5f, 0, new Vec3(30, 0.1f, 30));
            Material floorMaterial = new Material(Shader.FromFile("floor.hlsl"));

            floorMaterial.Transparency = Transparency.Blend;

            //ModelWrapper handModel = new VRHand("Hand_RIGHT_NoWrist.glb");
            ModelWrapper handModelRight = new VRHand(Handed.Right);
            HandJoint[] dataRight = Input.Hand(Handed.Right).fingers;

            ModelWrapper handModelLeft = new VRHand(Handed.Left);
            HandJoint[] dataLeft = Input.Hand(Handed.Left).fingers;


            bool toggleHand = true;
            bool renderHand = true;
            bool drawAxes = false;
            Pose windowPose = new Pose(0, 0.2f, -0.3f, Quat.LookDir(0, 0, 1));
            Quat rot = Quat.FromAngles(90, 0, 180);
            // Core application loop
            while (SK.Step(() =>
            {
                dataRight = Input.Hand(Handed.Right).fingers;
                dataLeft = Input.Hand(Handed.Left).fingers;

                UI.WindowBegin("Window", ref windowPose, new Vec2(20, 0) * U.cm);
                UI.Toggle(toggleHand ? "toggleHand" : "toggleHand", ref toggleHand);
                UI.Toggle(renderHand ? "renderHand" : "renderHand", ref renderHand);
                UI.Toggle(drawAxes ? "drawAxes" : "drawAxes", ref drawAxes);
                UI.WindowEnd();
                
                if (SK.System.displayType == Display.Opaque)
                    Default.MeshCube.Draw(floorMaterial, floorTransform);

              
               Input.HandVisible(Handed.Right, renderHand);
               Input.HandVisible(Handed.Left, renderHand);


                handModelRight.show(dataRight, drawAxes, toggleHand, Handed.Right);
                handModelLeft.show(dataLeft, drawAxes, toggleHand, Handed.Left);
                if (drawAxes)
                {
                    foreach (var item in dataRight)
                    {
                        Pose objPose = item.Pose;
                        // objPose.orientation = objPose.orientation * rot;

                        Lines.AddAxis(objPose);


                    }

                    foreach (var item in dataLeft)
                    {
                        Pose objPose = item.Pose;
                        //objPose.orientation = objPose.orientation * rot;

                        Lines.AddAxis(objPose);


                    }
                }
                




            })) ;
            SK.Shutdown();
        }
    }
}

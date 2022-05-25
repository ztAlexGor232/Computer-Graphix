﻿namespace Lab1
{
    public class Scene
    {
        private readonly Camera cam;
        private readonly DirectionalLight light;
        private readonly ITraceable[] objects;
        private float[] ViewValues;

        public Scene()
        {
            cam = new Camera(new Point(0, 0, -5), new Vector3D(0, 0, 1), 20, 20, 5);
            light = new DirectionalLight(new Point(10, 20, 0), new Vector3D(0, 1, 0.5f));
            objects = new ITraceable[] { new Plane(new Point(-2, 0, 1), new Point(2, 0, 1), new Point(0, -3, 2))};
            ViewValues = new float[cam.GetScreenHeight() * cam.GetScreenWidth()];
            ClearView();
        }

        public Scene(ITraceable[] objArr)
        {
            cam = new Camera(new Point(0, 0, 0), new Vector3D(0, 0, 1), 20, 20, 5);
            light = new DirectionalLight(new Point(10, 20, 0), new Vector3D(0, 1, 0.5f));
            objects = objArr;
            ViewValues = new float[cam.GetScreenHeight() * cam.GetScreenWidth()];
            ClearView();
        }

        private void ClearView()
        {
            for(int i = 0; i < ViewValues.Length; i++)
                ViewValues[i] = 0.0f;
        }

        public void RayProcessing()
        {
            int screenHeight = cam.GetScreenHeight();
            int screenWidth = cam.GetScreenWidth();
            Point camPosition = cam.GetPosition();
            Point screenNW = new(camPosition.X() - screenWidth / 2,
                                camPosition.Y() - screenHeight / 2,
                                camPosition.Z() + cam.GetFocalDistance());
            ClearView();

            for (int i = 0; i < screenHeight; i++)
            {
                for (int j = 0; j < screenWidth; j++)
                {
                    Beam ray = new(new Point(camPosition), new Vector3D(camPosition,
                        new Point(screenNW.X() + j, screenNW.Y() + i, screenNW.Z())));
                    ITraceable resObj;
                    Point? intersectionPoint = RayIntersect(ray, out resObj);
                    if(intersectionPoint is not null)
                        ViewValues[i * screenWidth + j] = -(resObj.GetNormalAtPoint(intersectionPoint) * light.GetDirection());
                }
            }
            ViewOutput();
        }

        public Point RayIntersect(Beam ray, out ITraceable intObj)
        {
            int depth = int.MaxValue;
            Point result = null;
            intObj = null;
            foreach (ITraceable obj in objects)
            {
                if(obj is not null) {
                    Point? intersectionPoint = obj.GetIntersectionPoint(ray);
                    if (intersectionPoint is not null)
                    {
                        Vector3D objNormal = obj.GetNormalAtPoint(intersectionPoint);
                        float dotProductValue = -(objNormal * light.GetDirection());
                        if (intersectionPoint.Z() < depth)
                        {
                            depth = (int)intersectionPoint.Z();
                            result = intersectionPoint;
                            intObj = obj;
                        }
                    }
                }
            }
            return result;
        }

        private void ViewOutput()
        {
            for (int i = 0; i < cam.GetScreenWidth(); i++)
            {
                for (int j = 0; j < cam.GetScreenWidth(); j++)
                {
                    float val = ViewValues[i * cam.GetScreenWidth() + j];

                    if (val <= 0)
                    {
                        Console.Write(' '.ToString().PadLeft(3));
                    }
                    else if (val > 0 && val < 0.2f)
                    {
                        Console.Write('·'.ToString().PadLeft(3));
                    }
                    else if (val >= 0.2f && val < 0.5f)
                    {
                        Console.Write('*'.ToString().PadLeft(3));
                    }
                    else if (val >= 0.5f && val < 0.8f)
                    {
                        Console.Write('O'.ToString().PadLeft(3));
                    }
                    else if (val >= 0.8f)
                    {
                        Console.Write('#'.ToString().PadLeft(3));
                    }
                }
                Console.WriteLine();
            }
        }
    }
}

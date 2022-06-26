﻿using System.Drawing;

namespace Lab1
{
    public class Light
    {
        protected float intensity;
        protected Color color;
        protected bool bIsRayInfinite;
        protected int generatedRaysNumber;
        protected float attenuationCoefficient;
        private readonly Random random;

        public Light(float intensity, Color color, int raysN = 16)
        {
            this.intensity = intensity;
            this.color = color;
            random = new Random();
            bIsRayInfinite = true;
            attenuationCoefficient = 1.0f;
            generatedRaysNumber = raysN;
        }

        public virtual Point? GetPosition() => null;

        public virtual Vector3D? GetDirection() => null;

        public void SetIntensity(float intensity) => this.intensity = intensity;

        public void SetColor(Color newColor) => color = newColor;

        public float GetIntensity() => intensity;

        public Color GetColor() => color;

        public virtual List<Vector3D> GetRayDirection(Vector3D n, Point p)
        {
            static List<float> GetFilledArray(int n, float step)
            {
                List<float> randAngles = new List<float>(n);
                for (int i = 0; i < n; i++)
                {
                    randAngles.Add(-89.0f + step * i);
                }
                return randAngles;
            }

            int raysNumberTiltedAroundX = generatedRaysNumber;
            int raysNumberTiltedAroundY = generatedRaysNumber;
            int raysNumberTiltedAroundZ = generatedRaysNumber;

            List<float> randAnglesX = GetFilledArray(raysNumberTiltedAroundX, 178.0f / raysNumberTiltedAroundX);
            List<float> randAnglesY = GetFilledArray(raysNumberTiltedAroundY, 178.0f / raysNumberTiltedAroundY);
            List<float> randAnglesZ = GetFilledArray(raysNumberTiltedAroundZ, 178.0f / raysNumberTiltedAroundZ);

            List<Vector3D> res = new List<Vector3D>(generatedRaysNumber + 1);

            for (int _ = 0; _ < generatedRaysNumber; _++)
            {
                int ix = random.Next(0, randAnglesX.Count);
                int iy = random.Next(0, randAnglesY.Count);
                int iz = random.Next(0, randAnglesZ.Count);
                res.Add(new Vector3D(randAnglesX[ix], randAnglesY[iy], randAnglesZ[iz]));
                randAnglesX.RemoveAt(ix);
                randAnglesY.RemoveAt(iy);
                randAnglesZ.RemoveAt(iz);
            }

            res.Add(new Vector3D(n));

            return res;
        }

        public virtual float GetAttenuationCoefficient(Point p) => attenuationCoefficient;

        public bool IsRayInfinite() => bIsRayInfinite;
    }

    public class DirectionalLight : Light
    {
        private Vector3D direction;

        public DirectionalLight(Vector3D direction, float intensity, Color color) : base(intensity, color, 1) 
        {
            this.direction = Vector3D.Normalize(direction);
        }

        public void SetDirection(Vector3D v) => direction = new Vector3D(v);

        public override Vector3D GetDirection() => direction;

        public override List<Vector3D> GetRayDirection(Vector3D n, Point p)
        {
            List<Vector3D> res = new List<Vector3D>(1);
            res.Add(-direction);
            return res;
        }

    }

    public class PointLight : Light
    {
        private Point position;

        public PointLight(Point position, float intensity, Color color) : base(intensity, color, 1)
        {
            this.position = new(position);
            bIsRayInfinite = false;
        }

        public void SetPosition(Point p) => position = new Point(p);

        public override Point GetPosition() => position;

        public override List<Vector3D> GetRayDirection(Vector3D n, Point p)
        {
            List<Vector3D> res = new List<Vector3D>(1);
            res.Add(new Vector3D(p, position));
            return res;
        }

        public override float GetAttenuationCoefficient(Point p) => 1.0f / new Vector3D(p, position).SquareLength();
    }
}

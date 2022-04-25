using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyek_UTS
{

    internal class Asset3d
    {

        // because many vertices 
        List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();

        // these should always be here
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;

        // for camera purposes

        Matrix4 _model; // to refer to the objects in this instance

        // math constants
        double _pi = Math.PI;

        // rotation purposes
        public Vector3 _centerPosition; // this is more convenient for center positions
        public Vector3 _tempPosition;
        public List<Vector3> _euler; // rotation matrices

        // scaling purposes
        float _currentScale = 1;

        // for joints
        public List<float> _bendList = new List<float>();

        // for color obv
        Vector3 _color;
        // composition
        public List<Asset3d> Child;

        //constructor
        public Asset3d(List<Vector3> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
            setDefault();
        }

        //constructor
        public Asset3d()
        {
            setDefault();
        }

        public Asset3d(Vector3 position)
        {
            setDefault();
            _centerPosition = position;
        }

        // PAM
        // setting default model options

        public void setDefault()
        {
            _model = Matrix4.Identity;
            setEuler();
            // center pos
            _centerPosition = new Vector3(0, 0, 0);

            //color 
            _color = new Vector3(0, 0, 0);

            // children
            Child = new List<Asset3d>();

            // vertices list
            _vertices = new List<Vector3>();
        }

        public void setEuler()
        {
            _euler = new List<Vector3>();

            // x y z
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);

            // xy, yz, xz
            _euler.Add(new Vector3(1, 1, 0));
            _euler.Add(new Vector3(0, 1, 1));
            _euler.Add(new Vector3(1, 0, 1));
            // xyz
            _euler.Add(new Vector3(1, 1, 1));
        }
        public void setCenterPosition(Vector3 centerPosition)
        {
            _centerPosition = centerPosition;
        }
        public void setColor(Vector3 color)
        {
            _color.X = color.X / 255f;
            _color.Y = color.Y / 255f;
            _color.Z = color.Z / 255f;
        }
        // load and render
        public void Load(string shadervert, string shaderfrag, float Size_x = 600, float Size_y = 600)
        {
            // buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count
                * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            // VAO
            _vertexArrayObject = GL.GenVertexArray();

            // load the arrays
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // if using indices, load these as well.
            if (_indices.Count != 0)
            {
                //element buffer for rectangle since you need multiple triangles i guess????
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count
                    * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw); ;
            }
            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();


            foreach (var item in Child)
            {
                item.Load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }
        public void Render(int renderCode, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);
            //_model = temp; // this when you want to use default function, turn off to use denzels rotate
            _shader.SetVector3("objectColor", _color);
            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);


            // if using indices, draw elements
            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                // draw ordinary
                if (renderCode == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                // draw circle since fan has curved edge
                else if (renderCode == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Count + 1));
                }
                // draw with lines
                else if (renderCode == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Count));
                }


            }

            foreach (var item in Child)
            {
                item.Render(renderCode, camera_view, camera_projection); // assuming that each item has already been instantiated, which in our case they are
            }
        }

        // aux trig functions
        float Sec(float rad)
        {
            return (float)(1 / Math.Cos(rad));
        }
        float Cosec(float rad)
        {
            return (float)(1 / Math.Sin(rad));
        }
        float Cot(float rad)
        {
            return (float)(1 / Math.Tan(rad));
        }

        // you know i think i UNDERSTAND HOW VECTOR3 WORKS NOW
        public Vector3 getGradient(Vector3 color1, Vector3 color2, float separator)
        {
            Vector3 gradient = color2 - color1;
            gradient /= separator;
            return gradient;
        }
        // with params
        public void createBoxVertices(Vector3 position, float length)
        {
            _centerPosition.X = position.X;
            _centerPosition.Y = position.Y;
            _centerPosition.Z = position.Z;

            Vector3 temp_vector;

            //Titik 1
            temp_vector.X = position.X - length / 2.0f;
            temp_vector.Y = position.Y + length / 2.0f;
            temp_vector.Z = position.Z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 2
            temp_vector.X = position.X + length / 2.0f;
            temp_vector.Y = position.Y + length / 2.0f;
            temp_vector.Z = position.Z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 3
            temp_vector.X = position.X - length / 2.0f;
            temp_vector.Y = position.Y - length / 2.0f;
            temp_vector.Z = position.Z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 4
            temp_vector.X = position.X + length / 2.0f;
            temp_vector.Y = position.Y - length / 2.0f;
            temp_vector.Z = position.Z - length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 5
            temp_vector.X = position.X - length / 2.0f;
            temp_vector.Y = position.Y + length / 2.0f;
            temp_vector.Z = position.Z + length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 6
            temp_vector.X = position.X + length / 2.0f;
            temp_vector.Y = position.Y + length / 2.0f;
            temp_vector.Z = position.Z + length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 7
            temp_vector.X = position.X - length / 2.0f;
            temp_vector.Y = position.Y - length / 2.0f;
            temp_vector.Z = position.Z + length / 2.0f;
            _vertices.Add(temp_vector);
            //Titik 8
            temp_vector.X = position.X + length / 2.0f;
            temp_vector.Y = position.Y - length / 2.0f;
            temp_vector.Z = position.Z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //Segitiga 
                0,1,2,
                1,2,3,
                0,4,5,
                0,1,5,
                1,3,5,
                3,5,7,
                0,2,4,
                2,4,6,
                4,5,6,
                5,6,7,
                2,3,6,
                3,6,7
            };
        }
        // use linestrips.

        // WITH SECTOR AND STACK COUNTS
        // flip v and u loops here.
        public void F_createTorus(Vector3 position, float radMajor, float radMinor, float sectorCount, float stackCount)
        {
            _centerPosition = position;

            float pi = (float)_pi;
            Vector3 tempVec;
            // stack = lintang, sector = b
            stackCount *= 2;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = 2 * pi / stackCount;
            float sectorAngle, stackAngle, tempX, tempY, tempZ;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                // making the circle i think
                tempX = radMajor + radMinor * (float)Math.Cos(stackAngle);
                tempY = radMinor * (float)Math.Sin(stackAngle);
                tempZ = radMajor + radMinor * (float)Math.Cos(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    tempVec.X = position.X + tempX * (float)Math.Cos(sectorAngle);
                    tempVec.Y = position.Y + tempY;
                    tempVec.Z = position.Z + tempZ * (float)Math.Sin(sectorAngle);

                    _vertices.Add(tempVec);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    _indices.Add(k1);
                    _indices.Add(k2);
                    _indices.Add(k1 + 1);

                    _indices.Add(k1 + 1);
                    _indices.Add(k2);
                    _indices.Add(k2 + 1);
                }
            }
        }
        public void F_createEllipsoid(Vector3 position, float radiusX, float radiusY, float radiusZ, int sectorCount, int stackCount)
        {
            _centerPosition = position;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = position.X + x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = position.Y + y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = position.Z + z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }
        public void F_createPartEllipsoid(Vector3 position, float radiusX, float radiusY, float radiusZ, int sectorCount, int stackCount, float part = 1)
        {
            _centerPosition = position;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = part * 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = position.X + x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = position.Y + y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = position.Z + z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }
        // has been modified so it only renders the part of cone we need.
        public void F_createCone(Vector3 position, float a, float b, float c, float sectorCount, float stackCount)
        {
            _centerPosition = position;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, stackAngle, x, y, z;

            // half here for best effect
            for (int v = 0; v <= stackCount / 2; ++v)
            {
                // use stackAngle for v
                stackAngle = pi / 2 - v * stackStep;
                x = a * stackAngle;
                y = b * stackAngle;
                z = c * stackAngle;

                for (int u = 0; u <= sectorCount; ++u)
                {
                    // use sectorAngle for u
                    sectorAngle = u * sectorStep;

                    temp_vector.X = position.X + x * (float)Math.Cos(sectorAngle); // a * v * cos(u)  
                    temp_vector.Y = position.Y + y * (float)Math.Sin(sectorAngle); // b * v * sin(u)  
                    temp_vector.Z = position.Z + z; // c * v  
                    _vertices.Add(temp_vector);
                }
            }


            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    // remove the limiters here to ensure all triangle sectors are rendered
                    //if (i != 0)
                    //{
                    _indices.Add(k1);
                    _indices.Add(k2);
                    _indices.Add(k1 + 1);
                    //}
                    //if (i != (stackCount - 1))
                    //{
                    _indices.Add(k1 + 1);
                    _indices.Add(k2);
                    _indices.Add(k2 + 1);
                    //}
                }
            }

        }

        // BEHOLD MY MAGNUM OPUS
        // i can make EVERYTHING
        public void F_createPrism(Vector3 position, float sides, float rBot, float rTop, float height)
        {
            _centerPosition = position;
            float step = 360 / sides; // the angular interval of each side
            Vector3 tempVec;

            // plot sides points
            for (float i = 0; i <= 360; i += step)
            {
                double degInRad = i * _pi / 180;

                // bottom
                tempVec.X = position.X + (float)Math.Cos(degInRad) * rBot;
                tempVec.Y = position.Y + (float)Math.Sin(degInRad) * rBot;
                tempVec.Z = position.Z;
                _vertices.Add(tempVec);
                // top
                tempVec.X = position.X + (float)Math.Cos(degInRad) * rTop;
                tempVec.Y = position.Y + (float)Math.Sin(degInRad) * rTop;
                tempVec.Z = position.Z + height;
                _vertices.Add(tempVec);
            }

            // add indices
            for (uint i = 0; i < _vertices.Count; i++)
            {
                if (i == _vertices.Count - 2)
                {
                    _indices.Add(i);
                    _indices.Add(i + 1);
                    _indices.Add(0);
                }
                else if (i == _vertices.Count - 1)
                {
                    _indices.Add(i);
                    _indices.Add(0);
                    _indices.Add(1);
                }
                else
                {
                    _indices.Add(i);
                    _indices.Add(i + 1);
                    _indices.Add(i + 2);
                }
            }
        }

        //rotations
        public Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;

            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));

            newPosition.Y =
                temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));

            newPosition.Z =
                temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            // turns angle to radians
            var radAngle = MathHelper.DegreesToRadians(angle);

            // https://en.wikipedia.org/wiki/Rotation_matrix
            // rotation matrix from axis and angle

            var arbRotationMatrix = new Matrix4
                (
                new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
                Vector4.UnitW
                );

            _model *= Matrix4.CreateTranslation(-pivot);
            _model *= arbRotationMatrix;
            _model *= Matrix4.CreateTranslation(pivot);

            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
            }

            _centerPosition = getRotationResult(pivot, vector, radAngle, _centerPosition);
            //rotationCenter = getRotationResult(pivot, vector, radAngle, rotationCenter);
            //objectCenter = getRotationResult(pivot, vector, radAngle, objectCenter);

            foreach (var i in Child)
            {
                i.rotate(pivot, vector, angle);
            }
        }

        // scaling
        public void scale(float factor)
        {
            _model *= Matrix4.CreateScale(factor / _currentScale); // factor is incremented by 0.01 per keyboard press
            foreach (var i in Child)
            {
                i.scale(factor);
            }
            // for next call
            _currentScale = factor;
        }
        // translating
        //coulda done this sooner
        public void translate(Vector3 position)
        {
            _model *= Matrix4.CreateTranslation(position);
            _centerPosition += position;

            foreach (var i in Child)
            {
                i.translate(position);
            }
        }

        public Vector3 getTranslationResult(Vector3 positionFrom, Vector3 positionTo)
        {
            return positionTo - positionFrom;
        }
        // reset
        public void reset()
        {
            _model = Matrix4.Identity;
            _currentScale = 1;
            _centerPosition = (0, 0, 0);
            setEuler();
            foreach (var i in Child)
            {
                i.reset();
            }
        }

        public void resetSelf()
        {
            _model = Matrix4.Identity;
            _currentScale = 1;
            _centerPosition = (0, 0, 0);
            setEuler();
        }

        // 赤ちゃんを作る
        public void addChild(Asset3d 赤ちゃん)
        {
            Child.Add(赤ちゃん);
        }

        // render complex objects here

        public Asset3d P_makeArmJoint(Vector3 center, Vector3 color, float scale = 0.1f, float length = 0.5f)
        {
            Asset3d armJoint = new Asset3d(center);
            armJoint.F_createEllipsoid(center, 0.5f * scale, 0.5f * scale, 0.5f * scale, 72, 24);
            armJoint.setColor(color);
            Asset3d arm = new Asset3d(center);
            arm.F_createPrism(center, 20, 0.5f * scale, 0.5f * scale, length);
            arm.setColor(color);
            armJoint.addChild(arm);

            return armJoint;
        }
        // for when want tip
        public Asset3d P_makeArmJoint(Asset3d tip, Vector3 center, Vector3 color, float scale = 0.1f)
        {
            Asset3d armJoint = new Asset3d(center);
            armJoint.F_createEllipsoid(center, 0.5f * scale, 0.5f * scale, 0.5f * scale, 72, 24);
            armJoint.setColor(color);
            armJoint.addChild(tip);

            return armJoint;
        }
        // remember that a joint's first child is its arm. the OTHER joint is its SECOND child.
        public Asset3d P_makeChain(Vector3 center, Vector3 color1, Vector3 color2, float scale = 0.1f, float length = 0.5f, int jNumber = 2)
        {
            Asset3d motherJoint = new Asset3d();
            motherJoint = motherJoint.P_makeArmJoint(center, color1, scale, length);
            Asset3d temp = new Asset3d();
            Asset3d childJoint = new Asset3d();

            // BEHOLD MY GRADIENT
            Vector3 gradient = getGradient(color1, color2, jNumber);
            // make chain
            for (int i = 1; i < jNumber; i++)
            {
                color1 += gradient;
                center.Z += length;
                if (i == 1)
                {
                    temp = temp.P_makeArmJoint(center, color1, scale, length);
                    motherJoint.addChild(temp);
                }
                else
                {
                    childJoint = childJoint.P_makeArmJoint(center, color1, scale, length);
                    temp.addChild(childJoint);
                    temp = childJoint;

                }
                childJoint = new Asset3d();
            }

            //return
            return motherJoint;
        }
        // i donot care anymore
        public Asset3d P_makeChain(Asset3d tip, Vector3 center, Vector3 color1, Vector3 color2, float scale = 0.1f, float length = 0.5f, int jNumber = 2)
        {
            Asset3d motherJoint = new Asset3d();
            motherJoint = motherJoint.P_makeArmJoint(center, color1, scale, length);
            Asset3d temp = new Asset3d();
            Asset3d childJoint = new Asset3d();
            // to ensure complete gradation to whatever was 0 before
            Vector3 gradient = getGradient(color1, color2, jNumber);
            // make chain
            for (int i = 1; i < jNumber; i++)
            {
                color1 += gradient;
                center.Z += length;
                if (i == 1)
                {
                    temp = temp.P_makeArmJoint(center, color1, scale, length);
                    motherJoint.addChild(temp);
                }
                else if (i == jNumber - 1)
                {
                    // make the flagel hand
                    tip.translate(tip.getTranslationResult(tip._centerPosition, center));
                    temp.addChild(tip);
                    // no need to continue temp because this is last
                }
                else
                {
                    childJoint = childJoint.P_makeArmJoint(center, color1, scale, length);
                    temp.addChild(childJoint);
                    temp = childJoint;

                }
                childJoint = new Asset3d();
            }

            //return
            return motherJoint;
        }

        // Peter - actions
        // curls n-segments of a joint x-degrees in total.
        // does this make it a bezier of sorts?
        // answer : it does not. it's neat tho.
        public List<float> A_setJointCurl(List<float> bendList, float totalDegree, int segments)
        {
            float period = totalDegree / segments;
            for (int i = 0; i < segments; i++)
            {
                bendList.Add(period);
            }
            return bendList;
        }

        // joint does a pseudo sin-wave, then gets normalized.
        public List<float> A_setJointWave(List<float> bendList, float waveDegree, int segments, int halfwaves = 1)
        {
            int halfWavePeriod = segments / halfwaves; // determine how many segments for one halfwave
            for (int i = 0; i < halfwaves; i++)
            {

                bendList = A_setJointCurl(bendList, waveDegree, halfWavePeriod);
                waveDegree *= -1;
            }
            // we put the reverse here or else >:(
            // since we're making a regular oscillation, to return it to the plane we must dampen the first joint's rotation by half.

            float half = -bendList[0] / 2;
            bendList[0] += half;
            return bendList;

        }

        // for getting a list of rotations necessary to turn a joint from degAlphas to degOmegas
        public List<float> A_setJointCurlDiff(List<float> bendList, float degAlpha, float degOmega, int segments)
        {
            return bendList;
        }

        // for 
        public List<float> A_setJointCurlDiffPerTime(List<float> bendList, float degAlpha, float degOmega, int segments, float duration)
        {
            return bendList;
        }
        // for lack of a better name
        public void A_furl(Asset3d item, int index, List<float> deg, int axis = 1)
        {
            // this ensures we only rotate the joints and not the tip... i hope
            if (index == 0)
            {

                item.rotate(item._centerPosition, item._euler[axis], deg[index]);
                return;
            }
            else
            {

                A_furl(item.Child[1], index - 1, deg, axis);
                item.rotate(item._centerPosition, item._euler[axis], deg[index]);

            }
        }

        // for backgrounds
        public Asset3d B_Terrain(Vector3 center, Vector3 color1, Vector3 color2, int layers = 3, float scale = 1)
        {
            Asset3d terrain = new Asset3d();
            Asset3d terrainLayer = new Asset3d();
            float initRadius = 2f;
            float step = initRadius / layers;
            Vector3 gradient = getGradient(color1, color2, layers);
            for (int i = 0; i < layers; i++)
            {

                // if last, make a cone
                if (i == layers - 1)
                {
                    terrainLayer.F_createPrism(center, 360, initRadius * scale, 0, 0.015f);
                    terrainLayer.setColor(color1);
                }
                // else just make a narrowing tube
                else
                {
                    terrainLayer.F_createPrism(center, 360, initRadius * scale, (initRadius - step) * scale, 0.015f);
                    terrainLayer.setColor(color1);
                }
                center += new Vector3(0, 0, 0.015f);
                initRadius -= step;
                color1 += gradient;
                terrain.addChild(terrainLayer);
                terrainLayer = new Asset3d();
            }
            terrain.rotate(terrain._centerPosition, terrain._euler[0], -90);
            return terrain;
        }

        public Asset3d B_makeTree(Vector3 center, Vector3 color1, Vector3 color2, float scale = 1f)
        {
            System.Random random = new System.Random();
            Asset3d tree = new Asset3d();
            Asset3d treeparts = new Asset3d();
            int leafLayers = random.Next(3, 7);
            Vector3 gradient = getGradient(color1, color2, leafLayers);

            // leaves
            Vector3 leafPos = center + new Vector3(0, 0.3f * scale, 0);
            for (int i = 0; i < leafLayers; i++)
            {
                treeparts.F_createPrism(leafPos, 10, 0.3f * scale, 0, 0.4f);
                treeparts.setColor(color1);
                treeparts.rotate(treeparts._centerPosition, treeparts._euler[0], -90);
                treeparts.rotate(treeparts._centerPosition, treeparts._euler[2], random.Next(0, 90));
                tree.addChild(treeparts);
                treeparts = new Asset3d();
                color1 += gradient;
                leafPos += new Vector3(0, 0.15f * scale, 0);
            }

            //stalk
            treeparts.F_createPrism(center, 10, 0.1f, 0.1f, 0.5f);
            treeparts.setColor(color1);
            treeparts.rotate(treeparts._centerPosition, treeparts._euler[0], -90);
            tree.addChild(treeparts);
            return tree;
        }
        public Asset3d B_makeForest(Vector3 center, Vector3 color1, Vector3 color2, float radius = 2f, int density = 2, float scale = 1f)
        {
            System.Random random = new System.Random();
            Asset3d forest = new Asset3d();
            Asset3d tree = new Asset3d();
            int spread;

            for (float z = 0.5f * scale; z <= radius * scale; z += 0.5f * scale)
            {
                for (int i = 0; i < density; i++)
                {
                    Vector3 treePos = center + new Vector3(0, 0, z);
                    tree = tree.B_makeTree(treePos, color1, color2);
                    spread = random.Next(0, 72);
                    tree.rotate(center, tree._euler[1], 5 * spread);
                    forest.addChild(tree);
                    tree = new Asset3d();
                }
                density += density;
            }

            return forest;
        }
    }
}

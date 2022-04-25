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
    
    internal class Asset2d
    {
        // this is an array, we're making 3 vertices
        float[] _vertices =
        {
            //x //y //z
        };
        uint[] _indices =
        {
           // the set of points you draw triangles among
        };
        int _mIndex;
        int[] _pascal = { };

        // these should always be here
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;

        //constructor
        public Asset2d(float[] vertices, uint[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        //constructor for circular affairs
        public Asset2d()
        {
            _vertices = new float[1080];
            _mIndex = 0;
        }

        //we're replicating the functions from window.cs
            // use settingCode to determine which attributes you use
        public void Load(string shadervert, string shaderfrag, int settingCode)
        {
            // buffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length
                * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            // VAO
            _vertexArrayObject = GL.GenVertexArray();

            // load the arrays
            GL.BindVertexArray(_vertexArrayObject);
            if (settingCode == 0)
            {
                //location 0, 3 elements, data type float, false, read 3 * size of float, offset 0
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);
            }
            else if(settingCode == 1)
            {
                // these are when you include the rgb with the vertices
                //arrays are 3 data every 6 elements
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                //// enable the above command
                GL.EnableVertexAttribArray(0);

                //colors are +3 from every 6 elements
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                // enable the above command
                GL.EnableVertexAttribArray(1);
            }

            // if using indices, load these as well.
            if(_indices.Length != 0)
            {
                //element buffer for rectangle since you need multiple triangles i guess????
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length
                    * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            }

            // this 
            //int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "ourColor");
            //GL.Uniform4(vertexColorLocation, 0.3f, 0.1f, 0.3f, 1.0f);


            // now we don't have to hardcode it.
            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();
        }

        public void Render(int _lines)
        {
            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);
           
            // if using indices, draw elements
            if(_indices.Length != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {   
                // draw ordinary
                if(_lines == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                }
                // draw circle since fan has curved edge
                else if (_lines == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Length + 1)/3);
                }
                // draw a line
                else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _mIndex);
                }
                // draw a bezier with the vertices
                else if(_lines == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Length/3));
                }

            }
        }

        // finally, 3d
        public void createCircle(float center_x, float center_y, float radius)
        {
            _vertices = new float[1080];
            // we have 360 points to draw this circle
            for (int i=0; i<360; i++)
            {
                // turn each i degree to radians
                double degInRad = i * Math.PI / 180;
                // we save in x, y, z

                //x 
                _vertices[i * 3] = radius * (float)Math.Cos(degInRad) + center_x;

                //y
                _vertices[i * 3 + 1] = radius * (float)Math.Sin(degInRad) + center_y;

                // z
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void createEllipse(float center_x, float center_y, float radius_x, float radius_y)
        {
            _vertices = new float[1080];
            // we have 360 points to draw this circle
            for (int i = 0; i < 360; i++)
            {
                // turn each i degree to radians
                double degInRad = i * Math.PI / 180;
                // we save in x, y, z

                //x 
                _vertices[i * 3] = radius_x * (float)Math.Cos(degInRad) + center_x;

                //y
                _vertices[i * 3 + 1] = radius_y * (float)Math.Sin(degInRad) + center_y;

                // z
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void updateMousePosition(float _x, float _y, float _z)
        {   
            // we're making a curvy construct with our own indices
            _vertices[_mIndex * 3] = _x;
            _vertices[_mIndex * 3 + 1] = _y;
            _vertices[_mIndex * 3 + 2] = _z;
            _mIndex++;

            // for telling the computer to remember all this
            GL.BufferData(BufferTarget.ArrayBuffer, _mIndex * 3 * sizeof(float),
                _vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public List<int> getRow(int rowIndex)
        {
            List<int> curRow = new List<int>();
            // it begins at 1
            curRow.Add(1);

            // if first order, is 1
            if(rowIndex == 0)
            {
                return curRow;
            }

            List<int> prev = getRow(rowIndex - 1);
            for(int i=1; i<prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                curRow.Add(curr);
            }
            //it ends at 1
            curRow.Add(1);

            return curRow;
        }

        public List<float> CreateCurveBezier()
        {
            List<float> _vertices_bezier = new List<float>();
            List<int> pascal = getRow(_mIndex - 1); // get pascal row of current number of dots - 1

            //start interpolating i guess\
            _pascal = pascal.ToArray();
            for(float t=0; t<=1.0f; t += 0.01f)
            {
                Vector2 p = getP(_mIndex, t);
                // update x y z
                _vertices_bezier.Add(p.X);
                _vertices_bezier.Add(p.Y);
                _vertices_bezier.Add(0);
            }

            return _vertices_bezier;
        }

        public Vector2 getP(int n, float t)
        {
            // the interpolacion
            Vector2 p = new Vector2(0, 0);
            float[] k = new float[n];

            for(int i=0; i<n; i++)
            {
                k[i] = (float)Math.Pow((1 - t), n - 1 - i) * (float)Math.Pow(t, i) * _pascal[i];
                p.X += k[i] * _vertices[i * 3];
                p.Y += k[i] * _vertices[i * 3 + 1];
            }
            return p;
        }

        //PAM
        public bool getVerticesLength()
        {
            if (_vertices[0] == 0)
                return false;

            if ((_vertices.Length + 1) / 3 > 0)
                return true;
            else 
                return false;
        }

        public void setVertices(float[] vertices)
        {
            _vertices = vertices;
        }
    }
}

using LearnOpenTK.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;



namespace Proyek_UTS
{
    internal class Window : GameWindow
    {
        // constants are much like java. you use a static class.
        static class Constants
        {
            // this is convenient
            public const string shaderPath = "../../../Shaders/";
            public static Vector3 defaultColor = new Vector3(0, 0, 0);
        }

        // related to transformations
        float scaleFactor = 1;

        // load models here temporarily.
        Camera cam;
        bool _firstMove = true;
        Vector2 _lastPos = new Vector2(0, 0);
        Vector3 _objectPost = new Vector3(0, 0, 0);
        float _rotationSpeed = 1f;

        Alien alien = new Alien();
        BulletBot bulletbot = new BulletBot();
        GigaBot gbot = new GigaBot();
        Asset3d mainObj = new Asset3d(new Vector3(0, 0, 0));
        Asset3d terrain = new Asset3d();
        Asset3d forest = new Asset3d();
        Asset3d gigabot = new Asset3d();
        Asset3d robot = new Asset3d();

        // time for animation
        int second = DateTime.Now.Second;
        int offset = 0;

        // positions
        Vector3 alienPos = new Vector3(0f, 0.85f, -0.8f);
        Vector3 botPos = new Vector3(-0.7f, 0.85f, 3.0f);
        Vector3 gigPos = new Vector3(0f, 0.45f, -1f);

        // yunus
        float yRunTimer = 0;

        // necessary constructor
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        // create and load objects here
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(.25f, .25f, .25f, 1.0f); //Background            




            gbot.setBody(gbot.H_makeGigaBot(gigPos, 1f));
            gbot._body.rotate(gbot._centerPosition, gbot._euler[1], 180);
            //gbot._body.translate(gigPos);

            bulletbot.setBody(bulletbot.Y_makeRobot(new Vector3(0, 0, 0), 0.5f));
            bulletbot._body.translate(botPos);
            bulletbot._body.rotate(mainObj._centerPosition, bulletbot._body._euler[1], -90f); // init position on the right side

            alien.setBody(alien.P_makeEntity(alienPos, new Vector3(0, 25, 50), new Vector3(255, 25, 50), 0.25f));
            //alien._body.translate(alienPos);

            terrain = terrain.B_Terrain(mainObj._centerPosition, new Vector3(0, 25, 50), new Vector3(200, 255, 5), 6, 2);
            forest = forest.B_makeForest(mainObj._centerPosition, new Vector3(0, 25, 50), new Vector3(200, 255, 5), 2, 1, 1.8f);

            mainObj.addChild(gbot._body);
            mainObj.addChild(alien._body);
            mainObj.addChild(bulletbot._body);
            mainObj.addChild(terrain);
            mainObj.addChild(forest);
            //gigabot.F_createCone(new Vector3(0, 0, 0), 0.6f, 0.6f, 0.6f, 24, 24);
            //mainObj.addChild(gigabot);
            mainObj.Load(Constants.shaderPath + "shader.vert", Constants.shaderPath + "shader.frag", Size.X, Size.Y);
            //mainObj.rotate(mainObj._centerPosition, mainObj._euler[0], 30);

            cam = new Camera(new Vector3(0, 0.5f, 3), Size.X / (float)Size.Y);
            CursorGrabbed = true;
        }
        // i am not good enough at math to understand this, but here it is
        public Matrix4 generateArbRotationMatrix(Vector3 axis, Vector3 center, float degree)
        {
            var rads = MathHelper.DegreesToRadians(degree);

            var secretFormula = new float[4, 4] {
                { (float)Math.Cos(rads) + (float)Math.Pow(axis.X, 2) * (1 - (float)Math.Cos(rads)), axis.X* axis.Y * (1 - (float)Math.Cos(rads)) - axis.Z * (float)Math.Sin(rads),    axis.X * axis.Z * (1 - (float)Math.Cos(rads)) + axis.Y * (float)Math.Sin(rads),   0 },
                { axis.Y * axis.X * (1 - (float)Math.Cos(rads)) + axis.Z * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Y, 2) * (1 - (float)Math.Cos(rads)), axis.Y * axis.Z * (1 - (float)Math.Cos(rads)) - axis.X * (float)Math.Sin(rads),   0 },
                { axis.Z * axis.X * (1 - (float)Math.Cos(rads)) - axis.Y * (float)Math.Sin(rads),   axis.Z * axis.Y * (1 - (float)Math.Cos(rads)) + axis.X * (float)Math.Sin(rads),   (float)Math.Cos(rads) + (float)Math.Pow(axis.Z, 2) * (1 - (float)Math.Cos(rads)), 0 },
                { 0, 0, 0, 1}
            };
            var secretFormulaMatix = new Matrix4
            (
                new Vector4(secretFormula[0, 0], secretFormula[0, 1], secretFormula[0, 2], secretFormula[0, 3]),
                new Vector4(secretFormula[1, 0], secretFormula[1, 1], secretFormula[1, 2], secretFormula[1, 3]),
                new Vector4(secretFormula[2, 0], secretFormula[2, 1], secretFormula[2, 2], secretFormula[2, 3]),
                new Vector4(secretFormula[3, 0], secretFormula[3, 1], secretFormula[3, 2], secretFormula[3, 3])
            );

            return secretFormulaMatix;
        }

        //rendering here
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 m_id = Matrix4.Identity;
            mainObj.Render(3, cam.GetViewMatrix(), cam.GetProjectionMatrix());


            //animations
            Anim("Y_Run", 1);
            //Anim("Y_Arm_Rotate", 1);
            Anim("Y_Spike_Rotate_Idle", 4);
            //Anim("Y_Spike_Throw", 4);
            Anim("P_Eye_RotateLR", 5);
            //Anim("P_Feet_Gloop");
            Anim("P_FlagelTip_Rotate", 10);
            Anim("P_Head_Bob", 1);
            Anim("P_Flagel_Rotate", 0.5f);
            Anim("H_Mid_Rotate");
            Anim("H_Head_Rotate");
            SwapBuffers();

            // test frame

            //second = DateTime.Now.Second;
            //offset++;
            //Console.WriteLine(second + " | " + offset);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            cam.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            cam.Fov -= e.OffsetY;
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // camera controls
            CamControl(args);

            // rotations
            RotateControl();

            // translating
            //TranslateControl();

            // scaling
            ScaleControl();

            // mouse controls
            MouseControl();

            // cam rotation
            CamRotate();

        }
        void Anim(string anim_id, float speedFactor = 1)
        {
            if (anim_id == "P_FlagelTip_Rotate")
            {
                alien.P_FlagelTip_RotateLoop(speedFactor);
            }
            else if (anim_id == "P_Flagel_Rotate")
            {
                alien.P_Flagel_BendLoop(alien._flagels, 90, 10);
            }
            else if (anim_id == "P_Eye_RotateLR")
            {
                alien.P_Eye_RotateLoop(speedFactor);
            }
            else if (anim_id == "P_Head_Bob")
            {
                alien.P_Head_Bob(speedFactor);
            }
            else if (anim_id == "Y_Run")
            {
                if (0 <= yRunTimer && yRunTimer < 144)
                {
                    bulletbot._body.rotate(mainObj._centerPosition, mainObj.Child[1]._euler[1], -2.5f * speedFactor);
                    yRunTimer++;
                }
                if (144 <= yRunTimer && yRunTimer < 244)
                {
                    bulletbot.Y_spikeBall_Throw(new Vector3(0, 0.5f, 0), yRunTimer, speedFactor);
                    yRunTimer++;
                }
                else if (yRunTimer >= 244)
                {
                    yRunTimer = 0;
                }
            }
            else if (anim_id == "Y_Arm_Rotate")
            {
                bulletbot._arm2.rotate(bulletbot._arm2._centerPosition, bulletbot._arm2._euler[1], 5);
            }
            else if (anim_id == "Y_Spike_Rotate_Idle")
            {
                //Console.WriteLine(bulletbot._spikeBall._centerPosition);
                bulletbot._spikeBalls.Child[0].rotate(bulletbot._spikeBalls.Child[0]._centerPosition, bulletbot._spikeBalls.Child[0]._euler[0], 2 * speedFactor);
                bulletbot._spikeBalls.Child[1].rotate(bulletbot._spikeBalls.Child[1]._centerPosition, bulletbot._spikeBalls.Child[1]._euler[0], 2 * speedFactor);
            }
            else if (anim_id == "Y_Spike_Throw")
            {
                bulletbot.Y_spikeBall_Throw(alienPos, speedFactor);
            }
            else if (anim_id == "H_Mid_Rotate")
            {
                //gbot._mid.rotate(gbot._mid._centerPosition, gbot._mid._euler[1], 5);
                gbot.H_moveMid();
            }
            else if (anim_id == "H_Head_Rotate")
            {
                gbot.H_moveFace();
                //gbot._face.rotate(gbot._face.Child[0]._centerPosition, gbot._face._euler[1], 5);
            }
        }
        void reset()
        {
            scaleFactor = 1;
            mainObj.reset();

        }
        void RotateControl(bool camera = false)
        {
            float inverse = 0.25f;
            if (camera)
            {
                inverse = -0.25f;
            }

            if (KeyboardState.IsKeyDown(Keys.Up))
            {
                mainObj.rotate(mainObj._centerPosition, mainObj._euler[0], inverse * -5);
            }
            if (KeyboardState.IsKeyDown(Keys.Down))
            {
                mainObj.rotate(mainObj._centerPosition, mainObj._euler[0], inverse * 5);
            }
            if (KeyboardState.IsKeyDown(Keys.Left))
            {
                mainObj.rotate(mainObj._centerPosition, mainObj._euler[1], inverse * -5);
            }
            if (KeyboardState.IsKeyDown(Keys.Right))
            {
                mainObj.rotate(mainObj._centerPosition, mainObj._euler[1], inverse * 5);
            }
            if (KeyboardState.IsKeyDown(Keys.Q))
            {
                mainObj.rotate(mainObj._centerPosition, mainObj._euler[2], inverse * -5);
            }
            if (KeyboardState.IsKeyDown(Keys.E))
            {
                mainObj.rotate(mainObj._centerPosition, mainObj._euler[2], inverse * 5);
            }
            if (KeyboardState.IsKeyDown(Keys.P))
            {
                reset();
            }
            Vector3 maju = mainObj._centerPosition + new Vector3(0, 0, 0.07f);
            if (KeyboardState.IsKeyDown(Keys.Y))
            {
                mainObj.Child[1].rotate(mainObj._centerPosition, mainObj.Child[1]._euler[1], -2.5f);
            }
            if (KeyboardState.IsKeyDown(Keys.U))
            {
                mainObj.Child[1].rotate(mainObj._centerPosition, mainObj.Child[1]._euler[1], 2.5f);
            }
            if (KeyboardState.IsKeyDown(Keys.H))
            {
                mainObj.Child[1].Child[1].Child[3].rotate(mainObj.Child[0].Child[1].Child[3]._centerPosition, Vector3.UnitX, 2.5f);
            }

        }
        void ScaleControl(bool camera = false)
        {
            int inverse = 1;
            if (camera)
            {
                inverse = -1;
            }

            if (KeyboardState.IsKeyDown(Keys.I))
            {
                if (scaleFactor < 5)
                {
                    scaleFactor += 0.01f;
                    mainObj.scale(scaleFactor);
                }
            }
            if (KeyboardState.IsKeyDown(Keys.O))
            {
                if (scaleFactor >= 0.2f)
                {
                    scaleFactor -= 0.01f;
                    mainObj.scale(scaleFactor);
                }
            }
        }
        void CamControl(FrameEventArgs args)
        {
            float cameraSpeed = 1f;
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                cam.Position += cam.Front * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                cam.Position -= cam.Front * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                cam.Position -= cam.Right * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                cam.Position += cam.Right * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                cam.Position += cam.Up * cameraSpeed * (float)args.Time;
            }
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
            {
                cam.Position -= cam.Up * cameraSpeed * (float)args.Time;
            }

        }
        void CamRotate()
        {
            if (KeyboardState.IsKeyDown(Keys.N))
            {
                var axis = new Vector3(0, 1, 0);
                cam.Position -= _objectPost;
                cam.Position = Vector3.Transform(
                    cam.Position,
                    generateArbRotationMatrix(axis, _objectPost, _rotationSpeed)
                    .ExtractRotation());
                cam.Position += _objectPost;
                cam._front = -Vector3.Normalize(cam.Position
                    - _objectPost);
            }
            if (KeyboardState.IsKeyDown(Keys.Comma))
            {
                var axis = new Vector3(0, 1, 0);
                cam.Position -= _objectPost;
                cam.Yaw -= _rotationSpeed;
                cam.Position = Vector3.Transform(cam.Position,
                    generateArbRotationMatrix(axis, _objectPost, -_rotationSpeed)
                    .ExtractRotation());
                cam.Position += _objectPost;

                cam._front = -Vector3.Normalize(cam.Position - _objectPost);
            }
            if (KeyboardState.IsKeyDown(Keys.K))
            {
                var axis = new Vector3(1, 0, 0);
                cam.Position -= _objectPost;
                cam.Pitch -= _rotationSpeed;
                cam.Position = Vector3.Transform(cam.Position,
                    generateArbRotationMatrix(axis, _objectPost, _rotationSpeed).ExtractRotation());
                cam.Position += _objectPost;
                cam._front = -Vector3.Normalize(cam.Position - _objectPost);
            }
            if (KeyboardState.IsKeyDown(Keys.M))
            {
                var axis = new Vector3(1, 0, 0);
                cam.Position -= _objectPost;
                cam.Pitch += _rotationSpeed;
                cam.Position = Vector3.Transform(cam.Position,
                    generateArbRotationMatrix(axis, _objectPost, -_rotationSpeed).ExtractRotation());
                cam.Position += _objectPost;
                cam._front = -Vector3.Normalize(cam.Position - _objectPost);
            }
        }
        void MouseControl()
        {
            var mouse = MouseState;
            var sensitivity = 0.2f;
            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);
                cam.Yaw += deltaX * sensitivity;
                cam.Pitch -= deltaY * sensitivity;
            }
        }
    }
}
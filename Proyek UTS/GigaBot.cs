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
    class GigaBot : Asset3d
    {
        // for ease of reference
        public Asset3d _body;
        public Asset3d _face;
        public Asset3d _head;
        public Asset3d _mid;
        public Asset3d _low;

        // for animation
        float spinTime = 0;
        float headSpinTime = 0;

        public GigaBot()
        {
            _body = new Asset3d();
            _face = new Asset3d();
            _head = new Asset3d();
            _mid = new Asset3d();
            _low = new Asset3d();
        }

        public void setBody(Asset3d body)
        {
            _body = body;
        }
        
        // animation
        public void H_moveMid()
        {
            if (0 <= spinTime && spinTime <= 72)
            {
                _mid.rotate(_mid._centerPosition, _mid._euler[1], 10);
                spinTime++;
            }
            else if (73 <= spinTime && spinTime < 74)
            {
                spinTime += 0.01f;
                spinTime = (float)Math.Round(spinTime, 2);
            }
            else if (74 <= spinTime && spinTime <= 146)
            {
                _mid.rotate(_mid._centerPosition, _mid._euler[1], -10);
                spinTime++;
            }
            else if (147 <= spinTime && spinTime < 148)
            {
                spinTime += 0.01f;
                spinTime = (float)Math.Round(spinTime, 2);
            }
            else
            {
                spinTime = 0;
            }
        }

        public void H_moveFace()
        {
            if (0 <= headSpinTime && headSpinTime <= 18)
            {
                _face.rotate(_face.Child[0]._centerPosition, _face._euler[1], 10);
                headSpinTime++;
            }
            else if (19 <= headSpinTime && headSpinTime < 20)
            {
                headSpinTime += 0.01f;
                headSpinTime = (float)Math.Round(headSpinTime, 2);
            }
            else if (20 <= headSpinTime && headSpinTime <= 38)
            {
                _face.rotate(_face.Child[0]._centerPosition, _face._euler[1], -10);
                headSpinTime++;
            }
            else if (39 <= headSpinTime && headSpinTime < 40)
            {
                headSpinTime += 0.01f;
                headSpinTime = (float)Math.Round(headSpinTime, 2);
            }
            else
            {
                headSpinTime = 0;
            }

        }
        // halim
        public Asset3d H_makeLimb(Vector3 center, bool isArm = false, float scale = 1)
        {
            Asset3d limb = new Asset3d();
            Asset3d ball = new Asset3d();
            limb.F_createPrism(center, 6, 0.25f * scale, 0.15f * scale, 0.6f * scale);
            limb.setColor(new Vector3(10, 10, 10));

            Vector3 ballPos = center + new Vector3(0, 0, 0.6f * scale);
            if (isArm)
            {
                ball.F_createCone(ballPos, 0.085f * scale, 0.085f * scale, 0.1f * scale, 24, 72);
                ball.rotate(ball._centerPosition, ball._euler[0], 180);
                ball.translate(new Vector3(0, 0, 0.08f));
            }
            else
            {
                ball.F_createPartEllipsoid(ballPos, 0.145f * scale, 0.10f * scale, 0.145f * scale, 24, 72, 0.5f);
                ball.rotate(ball._centerPosition, ball._euler[0], 90);
            }
            ball.setColor(new Vector3(255, 0, 0));
            limb.addChild(ball);
            return limb;
        }
        public Asset3d H_makeArm(Vector3 center, bool isArm = false, float scale = 1)
        {
            Asset3d arm = new Asset3d();
            Asset3d limb = new Asset3d();
            limb = H_makeLimb(center, isArm, scale);
            arm = arm.P_makeArmJoint(limb, center, new Vector3(20, 20, 20), 0.1f * scale);
            return arm;
        }
        public Asset3d H_makeSegmentHead(Vector3 center, float scale = 1, int slant = 0)
        {
            Asset3d segment = new Asset3d();
            // buletan tengah
            segment.F_createEllipsoid(center, 0.3f * scale, 0.3f * scale, 0.3f * scale, 24, 72);

            // muka
            Asset3d face = new Asset3d();
            Vector3 headPos = center + new Vector3(0, 0, 0.07f * scale);
            face = H_makeHead(headPos, scale);
            segment.addChild(face);

            // 2 tangan, kiri dan kanan
            Vector3 armPos = center + new Vector3(0, 0, 0.3f * scale);
            Asset3d arm = new Asset3d();
            arm = H_makeArm(armPos, false, scale);
            arm.rotate(center, arm._euler[1], 90);
            arm.rotate(center, arm._euler[0], slant);
            segment.addChild(arm);

            arm = new Asset3d();
            arm = H_makeArm(armPos, false, scale);
            arm.rotate(center, arm._euler[1], 270);
            arm.rotate(center, arm._euler[0], slant);
            segment.addChild(arm);

            return segment;
        }
        public Asset3d H_makeSegment(Vector3 center, bool isArm = false, float scale = 1, int slant = 0)
        {
            Asset3d segment = new Asset3d();
            // buletan tengah
            segment.F_createEllipsoid(center, 0.3f * scale, 0.3f * scale, 0.3f * scale, 24, 72);

            // 2 tangan, kiri dan kanan
            Vector3 armPos = center + new Vector3(0, 0, 0.3f * scale);
            Asset3d arm = new Asset3d();
            arm = H_makeArm(armPos, isArm, scale);
            arm.rotate(center, arm._euler[1], 90);
            arm.rotate(center, arm._euler[0], slant);
            segment.addChild(arm);

            arm = new Asset3d();
            arm = H_makeArm(armPos, isArm, scale);
            arm.rotate(center, arm._euler[1], 270);
            arm.rotate(center, arm._euler[0], slant);
            segment.addChild(arm);

            return segment;
        }
        public Asset3d H_makeGigaBot(Vector3 center, float scale = 1)
        {
            Asset3d bot = new Asset3d();
            Asset3d segment = new Asset3d();
            Vector3 offset = new Vector3(0, 0.3f * scale, 0);
            // bawah
            segment = H_makeSegment(center, false, 0.5f, 45);
            _low = segment;
            center += offset;
            bot.addChild(segment);
            // tengah
            segment = new Asset3d();
            segment = H_makeSegment(center, true, 0.5f, 0);
            _mid = segment;
            center += offset;
            bot.addChild(segment);
            //atas
            segment = new Asset3d();
            segment = H_makeSegmentHead(center, 0.5f, -45);
            //segment.Child[0]._centerPosition = center;
            _head = segment;
            _face = segment.Child[0]; // face is the first child
            bot.addChild(segment);

            return bot;
        }
        public Asset3d H_makeEyes(Vector3 center, Vector3 putih, Vector3 hitam, float scale = 1)
        {

            Asset3d eye = new Asset3d();
            Asset3d pupil = new Asset3d();
            // putih mata
            eye.F_createEllipsoid(center, 0.05f * scale, 0.07f * scale, 0.01f * scale, 24, 72);
            eye.setColor(putih);

            // hitam mata
            Vector3 pupilPos = center + new Vector3(0, 0, 0.01f * scale);
            pupil.F_createEllipsoid(pupilPos, 0.02f * scale, 0.02f * scale, 0.01f * scale, 24, 72);
            pupil.setColor(hitam);
            eye.addChild(pupil);
            return eye;
        }
        public Asset3d H_yellowFace(Vector3 center, float scale = 1)
        {
            Asset3d kuning = new Asset3d();
            Asset3d mata = new Asset3d();

            kuning.F_createPartEllipsoid(center, 0.25f * scale, 0.25f * scale, 0.25f * scale, 24, 72, 0.5f);
            kuning.setColor(new Vector3(255, 255, 0));
            kuning.rotate(kuning._centerPosition, kuning._euler[0], 90);

            Vector3 mataPos = center + new Vector3(0, 0, 0.25f * scale);
            mata = H_makeEyes(mataPos, new Vector3(255, 255, 255), new Vector3(0, 0, 0), scale);
            mata.rotate(kuning._centerPosition, mata._euler[1], -15);
            kuning.addChild(mata);
            mata = new Asset3d();
            mata = H_makeEyes(mataPos, new Vector3(255, 255, 255), new Vector3(0, 0, 0), scale);
            mata.rotate(kuning._centerPosition, mata._euler[1], 15);
            kuning.addChild(mata);

            return kuning;
        }
        public Asset3d H_redFace(Vector3 center, float scale = 1)
        {
            Asset3d kuning = new Asset3d();
            Asset3d mata = new Asset3d();

            kuning.F_createPartEllipsoid(center, 0.25f * scale, 0.25f * scale, 0.25f * scale, 24, 72, 0.5f);
            kuning.setColor(new Vector3(255, 0, 0));
            kuning.rotate(kuning._centerPosition, kuning._euler[0], 90);

            Vector3 mataPos = center + new Vector3(0, 0, 0.25f * scale);
            mata = H_makeEyes(mataPos, new Vector3(0, 0, 0), new Vector3(255, 0, 0), scale);
            mata.rotate(kuning._centerPosition, mata._euler[1], -15);
            kuning.addChild(mata);
            mata = new Asset3d();
            mata = H_makeEyes(mataPos, new Vector3(0, 0, 0), new Vector3(255, 0, 0), scale);
            mata.rotate(kuning._centerPosition, mata._euler[1], 15);
            kuning.addChild(mata);

            return kuning;
        }
        public Asset3d H_makeHead(Vector3 center, float scale = 1)
        {
            Asset3d kepala = new Asset3d();
            // setengah merah, setengah kuning
            Asset3d merah = new Asset3d();
            merah = H_redFace(center, scale);
            Asset3d kuning = new Asset3d();
            kuning = H_yellowFace(center, scale);
            kuning.rotate(kuning._centerPosition, kuning._euler[0], 180);
            kepala.addChild(merah);
            kepala.addChild(kuning);
            //kepala.rotate(kepala._centerPosition, kepala._euler[0], 90);

            return kepala;
        }
    }
}

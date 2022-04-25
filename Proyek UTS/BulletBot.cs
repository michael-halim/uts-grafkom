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
    class BulletBot : Asset3d
    {
        // for easier referencing
        public Asset3d _body; //contains the whole object
        public Asset3d _eye;
        public Asset3d _head;
        public Asset3d _torso;
        public Asset3d _arm1;
        public Asset3d _arm2;
        public Asset3d _spikeBall;
        public Asset3d _spikeBalls;


        // animation states
        Vector3 ballPosition;
        float ballTimer = 0;
        bool ballThrow = true;
        // mbencekno
        bool isSpikeBallPosition = false;


        // PAM
        public BulletBot()
        {
            _body = new Asset3d();
            _eye = new Asset3d();
            _head = new Asset3d();
            _torso = new Asset3d();
            _arm1 = new Asset3d();
            _arm2 = new Asset3d();
            _spikeBalls = new Asset3d();
            _spikeBall = new Asset3d();


        }

        public void setBody(Asset3d body)
        {
            _body = body;
        }

        // yunus
        public void Y_spikeBall_Throw(Vector3 target, float timer, float scale = 1f)
        {
            if (!isSpikeBallPosition)
            {
                ballPosition = _spikeBalls.Child[0]._centerPosition;
                isSpikeBallPosition = true;
            }

            // state
            if (ballTimer == 50)
            {
                
                ballThrow = false;
            }
            else if (ballTimer == 0)
            {
                ballThrow = true;
                isSpikeBallPosition = false;
            }

            // thing
            if (ballThrow == true)
            {
                Vector3 tVector = getTranslationResult(ballPosition, target) * 0.02f * scale;
                tVector.X = (float)Math.Round(tVector.X, 2);
                tVector.Y = (float)Math.Round(tVector.Y, 2);
                tVector.Z = (float)Math.Round(tVector.Z, 2);
                _spikeBalls.Child[0].translate(tVector);
                ballTimer += scale;
                
            }
            else
            {
                Vector3 tVector = getTranslationResult(target, ballPosition) * 0.02f * scale;
                tVector.X = (float)Math.Round(tVector.X, 2);
                tVector.Y = (float)Math.Round(tVector.Y, 2);
                tVector.Z = (float)Math.Round(tVector.Z, 2);
                _spikeBalls.Child[0].translate(tVector);
                ballTimer -= scale;
                
            }
            
        }
        public Asset3d Y_makeEye(Vector3 center, float radius)
        {
            _eye = new Asset3d();
            _eye.F_createEllipsoid(center, radius, 0.1f * radius, radius, 72, 24);
            _eye.setColor(new Vector3(255, 255, 255)); // white
            Asset3d eyeball = new Asset3d();

            center.Y += 0.1f * radius; // to make the origin point in front of it
            eyeball.F_createEllipsoid(center, 0.5f * radius, 0.005f * radius, 0.5f * radius, 72, 24);
            eyeball.setColor(new Vector3(0, 0, 0));
            _eye.addChild(eyeball);
            return _eye;
        }
        public Asset3d Y_makeArm(Vector3 center, float scale = 1)
        {
            Asset3d bahu = new Asset3d();
            Asset3d lengan = new Asset3d();
            Asset3d tanganTemp = new Asset3d();
            Asset3d tangan = new Asset3d();

            // lingkaran
            bahu.F_createEllipsoid(center, 0.25f * scale, 0.25f * scale, 0.25f * scale, 24, 72);
            // lengan 
            lengan = lengan.P_makeArmJoint(center, new Vector3(0, 0, 0), 0.2f * scale, 0.5f * scale);
            center += new Vector3(0, 0, 0.5f * scale);
            tanganTemp = Y_makeTangan(center, new Vector3(0, 0, 0), new Vector3(255, 0, 0), scale);
            tangan = tangan.P_makeArmJoint(tanganTemp, center, new Vector3(0, 0, 0));
            lengan.addChild(tangan);
            bahu.addChild(lengan);

            return bahu;
        }
        public Asset3d Y_makeTangan(Vector3 center, Vector3 color1, Vector3 color2, float scale = 1f)
        {
            Asset3d tangan = new Asset3d();
            Asset3d segment = new Asset3d();
            // untuk mendapatkan warna gradien
            Vector3 gradient = getGradient(color1, color2, 4);

            // alas
            segment.F_createPrism(center, 8, 0.1f * scale, 0, 0);
            segment.setColor(color1);
            tangan.addChild(segment);

            // segmen 1
            segment = new Asset3d();
            segment.F_createPrism(center, 8, 0.1f * scale, 0.2f * scale, 0.2f);
            segment.setColor(color1);
            tangan.addChild(segment);

            // segmen 2
            color1 += gradient;
            center.Z += 0.2f;
            segment = new Asset3d();
            segment.F_createPrism(center, 8, 0.2f * scale, 0.2f * scale, 0.2f);
            segment.setColor(color1);
            tangan.addChild(segment);

            // segmen 3
            color1 += gradient;
            center.Z += 0.2f;
            segment = new Asset3d();
            segment.F_createPrism(center, 8, 0.2f * scale, 0.2f * scale, 0.2f);
            segment.setColor(color1);
            tangan.addChild(segment);

            // bola
            center.Z += 0.2f;
            segment = new Asset3d();
            segment = Y_makeSpikeBall(center, color1, scale);
            tangan.addChild(segment);
            _spikeBalls.addChild(segment);

            return tangan;
        }
        public Asset3d Y_makeDada(Vector3 center, Vector3 color, float scale = 1f)
        {
            Asset3d dada = new Asset3d();
            dada.F_createEllipsoid(center, 0.7f * scale, 0.7f * scale, 0.7f * scale, 72, 24);
            dada.setColor(color);
            return dada;
        }
        public Asset3d Y_makePerut(Asset3d roda, Vector3 center, Vector3 color1, Vector3 color2, float scale = 1f)
        {
            Asset3d perut = new Asset3d();
            perut = perut.P_makeChain(roda, center, color1, color2, 0.5f * scale, 0.2f * scale, 6);
            perut.rotate(perut._centerPosition, perut._euler[0], 105);
            List<float> bendList = new List<float>();
            bendList = perut.A_setJointCurl(bendList, -15, 6);
            perut.A_furl(perut, bendList.Count - 1, bendList, 0);
            return perut;
        }
        public Asset3d Y_makeBody(Vector3 center, Vector3 colorDada, Vector3 colorPerut1, Vector3 colorPerut2, float scale = 1f)
        {
            _torso = new Asset3d();
            Asset3d dada = new Asset3d();
            Asset3d perut = new Asset3d();
            Asset3d kepala = new Asset3d();
            Asset3d roda = new Asset3d();

            // dada
            dada = Y_makeDada(center, colorDada, scale);
            center += new Vector3(0, 0.65f * scale, 0.3f * scale);
            kepala = Y_makeHead(center, new Vector3(20, 20, 20), scale);
            // putar berlawanan putaran badan
            kepala.rotate(kepala._centerPosition, kepala._euler[1], -45);
            dada.addChild(kepala);

            Vector3 centerPerut = center + new Vector3(0, (-0.65f - 0.65f) * scale, -0.3f * scale);

            // roda
            roda.F_createEllipsoid(center, scale * 0.5f, scale * 0.5f, scale * 0.5f, 72, 24);

            // perut
            perut = Y_makePerut(roda, centerPerut, colorPerut1, colorPerut2, scale);
            dada.rotate(perut._centerPosition, dada._euler[0], 15);

            _torso.addChild(dada);
            _torso.addChild(perut);


            //roda.F_createTorus();

            return _torso;
        }
        public Asset3d Y_makeHead(Vector3 center, Vector3 color, float scale = 1f)
        {
            _head = new Asset3d();
            _head.F_createPartEllipsoid(center, 0.3f * scale, 0.3f * scale, 0.3f * scale, 72, 24, 0.66f);
            _head.rotate(_head._centerPosition, _head._euler[1], 90);
            _head.setColor(color);

            _eye = new Asset3d();
            center.Z += 0.3f * scale;
            _eye = Y_makeEye(center, 0.06f * scale);
            _eye.setColor(new Vector3(255, 0, 0));
            _eye.rotate(_eye._centerPosition, _eye._euler[0], 90);
            _head.addChild(_eye);
            return _head;
        }
        public Asset3d Y_makeRobot(Vector3 center, float scale = 1f)
        {

            _body = new Asset3d();
            _torso = new Asset3d();
            Asset3d lengan = new Asset3d();

            _torso = Y_makeBody(center, new Vector3(60, 30, 60), new Vector3(40, 40, 40), new Vector3(0, 0, 0), scale);
            Vector3 pLengan = _torso._centerPosition + new Vector3(0, 0, 0.8f * scale);



            // lengan kanan, menghadap tengah rotasi
            lengan = Y_makeArm(pLengan, scale);
            lengan.rotate(_torso._centerPosition, lengan._euler[1], -90);
            lengan.rotate(_torso._centerPosition, lengan._euler[0], -35f);
            lengan.rotate(lengan._centerPosition, lengan._euler[0], 45f);
            lengan.rotate(lengan._centerPosition, lengan._euler[1], 25f);
            _torso.addChild(lengan);
            _arm1 = lengan;

            // lengan kiri, pose diatur
            lengan = new Asset3d();
            lengan = Y_makeArm(pLengan, scale);
            lengan.rotate(_torso._centerPosition, lengan._euler[1], 90);
            lengan.rotate(_torso._centerPosition, lengan._euler[0], -35f);
            lengan.rotate(lengan._centerPosition, lengan._euler[0], 90f);
            _arm2 = lengan;


            // rotasi sikut           
            lengan.Child[0].Child[1].rotate(lengan.Child[0].Child[1]._centerPosition, lengan.Child[0].Child[1]._euler[0], -45);
            lengan.Child[0].Child[1].rotate(lengan.Child[0].Child[1]._centerPosition, lengan.Child[0].Child[1]._euler[1], -90);
            _torso.addChild(lengan);

            // putar body
            _torso.rotate(_torso._centerPosition, _torso._euler[1], 35);

            // sesuaikan posisi kepalanya
            _body.addChild(_torso);
            _body.translate(new Vector3(0, 0.3f, -0.5f));
            _body.rotate(_body._centerPosition, _body._euler[1], 180);
            ballPosition = _spikeBalls.Child[0]._centerPosition;
            return _body;
        }
        public Asset3d Y_makeSpikeBall(Vector3 center, Vector3 color, float scale = 1)
        {
            Asset3d spikeball = new Asset3d();
            Asset3d spike = new Asset3d();
            spikeball.F_createEllipsoid(center, 0.25f * scale, 0.25f * scale, 0.25f * scale, 72, 24);
            Vector3 spikeCenter = center += new Vector3(0, 0, 0.23f * scale);
            //4 duri
            for (int i = 0; i < 360; i += 90)
            {
                spike.F_createPrism(spikeCenter, 24, 0.1f * scale, 0, 0.1f * scale);
                spike.setColor(color);
                spike.rotate(spikeball._centerPosition, spike._euler[1], i);
                spikeball.addChild(spike);
                spike = new Asset3d();
            }

            // satu duri diatas
            spike.F_createPrism(spikeCenter, 24, 0.1f * scale, 0, 0.1f * scale);
            spike.rotate(spikeball._centerPosition, spike._euler[0], 90);
            spike.setColor(color);
            spikeball.addChild(spike);
            spike = new Asset3d();

            // satu duri dibawah
            spike.F_createPrism(spikeCenter, 24, 0.1f * scale, 0, 0.1f * scale);
            spike.rotate(spikeball._centerPosition, spike._euler[0], 270);
            spike.setColor(color);
            spikeball.addChild(spike);

            // sesuaikan posisi
            spikeball.rotate(spikeball._centerPosition, spikeball._euler[0], -90);
            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(spikeball._euler[i]);
            }
            return spikeball;
        }
    }
}
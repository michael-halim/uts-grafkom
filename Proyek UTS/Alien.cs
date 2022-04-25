using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Proyek_UTS
{
    class Alien : Asset3d
    {
        // for easier referencing
        public Asset3d _body; //contains the whole object
        public Asset3d _head;
        public Asset3d _eye;
        public Asset3d _legs;
        public Asset3d _flagels;
        public Asset3d _flageltips;


        // necessary parameters for animation
        float _eyeRotDegree = 0;
        float _flageltipRotDegree = 0;
        Vector3 _headBobVector = new Vector3(0, 0, 0);
        float _headBobValue = 0;
        float _flagelTotalDegree = 90;
        float _eyeStopTimer = 0;
        bool _flageltipRot = true;
        bool _eyeRotPlus = true;
        bool _headBobPlus = true;
        bool _flagelPositive = true;

        //PAM
        public Alien()
        {
            _body = new Asset3d();
            _head = new Asset3d();
            _eye = new Asset3d();
            _legs = new Asset3d();
            _flagels = new Asset3d();
            _flageltips = new Asset3d();
        }
        public void setBody(Asset3d body)
        {
            _body = body;
        }

        // animations
        public void P_Head_Bob(float translateSpeed = 1)
        {
            _headBobVector = new Vector3(0, 0.001f * translateSpeed, 0);
            // state
            if (_headBobValue > 0.02f)
            {

                _headBobPlus = false;
            }
            else if (_headBobValue < -0.01f)
            {

                _headBobPlus = true;
            }

            // transform
            if (_headBobPlus)
            {
                _head.translate(_headBobVector);
                _headBobValue += 0.001f;
            }
            else
            {
                _head.translate(-_headBobVector);
                _headBobValue -= 0.001f;
            }
        }
        public void P_Eye_RotateLoop(float rotateSpeed = 1)
        {
            // deciding eyerotation 
            if (_eyeRotDegree >= 45)
            {
                if (_eyeStopTimer < 60)
                {
                    _eyeStopTimer++;
                }
                else
                {
                    _eyeRotPlus = false;
                    _eyeStopTimer = 0;
                }
                
            }
            else if (_eyeRotDegree <= -45)
            {
                if (_eyeStopTimer < 60)
                {
                    _eyeStopTimer++;
                }
                else
                {
                    _eyeRotPlus = true;
                    _eyeStopTimer = 0;
                }

            }
            

            if(_eyeStopTimer == 0)
            {
                if (_eyeRotPlus)
                {
                    _eye.rotate(_eye._centerPosition, _eye._euler[2], rotateSpeed);
                    _eyeRotDegree += rotateSpeed;
                }
                else
                {
                    _eye.rotate(_eye._centerPosition, _eye._euler[2], -rotateSpeed);
                    _eyeRotDegree -= rotateSpeed;
                }
            }
           
        }
        public void P_FlagelTip_RotateLoop(float rotateSpeed = 1)
        {
            // state
            if (_flageltipRotDegree >= 720)
            {
                _flageltipRot = false;
            }
            else if (_flageltipRotDegree < 0)
            {
                _flageltipRot = true;
            }

            // rotate
            if (_flageltipRot)
            {
                foreach (var child in _flageltips.Child)
                {
                    child.rotate(child._centerPosition, child._euler[2], rotateSpeed);
                }
                _flageltipRotDegree += rotateSpeed;
            }
            else
            {
                _flageltipRotDegree -= 2 * rotateSpeed;
            }

        }

        // this takes account the flagel in question
        public void P_Flagel_BendLoop(Asset3d flagels, float totalDegree = 90, float rotateSpeed = 1)
        {
            // state
            if (_flagelTotalDegree >= totalDegree)
            {
                _flagelPositive = true;
            }
            else if (_flagelTotalDegree <= -totalDegree)
            {
                _flagelPositive = false;
            }

            // transform
            List<float> bendList = new List<float>();
            if (_flagelPositive)
            {
                // towards -90
                bendList = A_setJointCurl(bendList, -rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, -rotateSpeed, 12);
                bendList.Reverse();
                flagels.Child[0].A_furl(flagels.Child[0], bendList.Count - 1, bendList, 0);
                
                
                // normalization rotation
                flagels.Child[0].rotate(flagels.Child[0]._centerPosition, flagels.Child[0]._euler[0], 0.5f * rotateSpeed);
                
                


                // the other flagel
                bendList.Clear();
                bendList = A_setJointCurl(bendList, rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, -rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, rotateSpeed, 12);
                bendList.Reverse();
                //flagels.Child[1].A_furl(flagels.Child[1], bendList.Count - 1, bendList, 0);
                flagels.Child[1].A_furl(flagels.Child[1], bendList.Count - 1, bendList, 1);
                // normalization rotation
                //flagels.Child[1].rotate(flagels.Child[1]._centerPosition, flagels.Child[1]._euler[0], -0.5f * rotateSpeed);
                flagels.Child[1].rotate(flagels.Child[1]._centerPosition, flagels.Child[1]._euler[1], -0.5f * rotateSpeed);

                _flagelTotalDegree -= rotateSpeed;
            }
            else
            {
                // towards 90
                bendList = A_setJointCurl(bendList, rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, -rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, rotateSpeed, 12);
                bendList.Reverse();
                flagels.Child[0].A_furl(flagels.Child[0], bendList.Count - 1, bendList, 0);
                
                
                // normalization rotation
                flagels.Child[0].rotate(flagels.Child[0]._centerPosition, flagels.Child[0]._euler[0], -0.5f * rotateSpeed);
                
                


                // the other flagel
                bendList.Clear();
                bendList = A_setJointCurl(bendList, -rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, rotateSpeed, 12);
                bendList = A_setJointCurl(bendList, -rotateSpeed, 12);
                bendList.Reverse();
                flagels.Child[1].A_furl(flagels.Child[1], bendList.Count - 1, bendList, 1);
                //flagels.Child[1].A_furl(flagels.Child[1], bendList.Count - 1, bendList, 0);
                // normalization rotation
                flagels.Child[1].rotate(flagels.Child[1]._centerPosition, flagels.Child[1]._euler[1], 0.5f * rotateSpeed);
                //flagels.Child[1].rotate(flagels.Child[1]._centerPosition, flagels.Child[1]._euler[0], 0.5f * rotateSpeed);

                _flagelTotalDegree += rotateSpeed;
            }
        }
        // components
        public Asset3d P_makeEye(Vector3 center, float radius = 1)
        {
            _eye = new Asset3d();
            _eye.F_createEllipsoid(center, radius, radius, radius, 72, 24);
            _eye.setColor(new Vector3(255, 255, 255)); // white
            Asset3d eyeball = new Asset3d();

            center.Y += radius; // to make the origin point in front of it
            eyeball.F_createEllipsoid(center, 0.2f * radius, 0.005f * radius, 0.2f * radius, 72, 24);
            eyeball.setColor(new Vector3(0, 0, 0));
            _eye.addChild(eyeball);
            return _eye;
        }
        public Asset3d P_makeLegs(Vector3 center, Vector3 legColor1, Vector3 legColor2, float scale = 1f)
        {
            Asset3d legs = new Asset3d(center);
            Asset3d leg = new Asset3d();
            List<float> bendList = new List<float>();
            bendList = leg.A_setJointCurl(bendList, 120, 18);
            bendList = leg.A_setJointCurl(bendList, -120, 9);

            leg._bendList = bendList;
            leg = leg.P_makeChain(legs._centerPosition, legColor1, legColor2, scale * 0.1f, scale * 0.05f, 27);
            leg.A_furl(leg, bendList.Count - 1, bendList);
            leg.rotate(leg._centerPosition, leg._euler[0], 90);
            legs.addChild(leg);
            leg = new Asset3d();

            leg._bendList = bendList;
            leg = leg.P_makeChain(legs._centerPosition, legColor1, legColor2, scale * 0.1f, scale * 0.05f, 27);
            leg.A_furl(leg, bendList.Count - 1, bendList);
            leg.rotate(leg._centerPosition, leg._euler[0], 90);
            leg.rotate(leg._centerPosition, leg._euler[2], 120);
            legs.addChild(leg);
            leg = new Asset3d();

            leg._bendList = bendList;
            leg = leg.P_makeChain(legs._centerPosition, legColor1, legColor2, scale * 0.1f, scale * 0.05f, 27);
            leg.A_furl(leg, bendList.Count - 1, bendList);
            leg.rotate(leg._centerPosition, leg._euler[0], 90);
            leg.rotate(leg._centerPosition, leg._euler[2], 240);
            legs.addChild(leg);

            return legs;
        }
        public Asset3d P_makeCompleteFlagel(Vector3 center, Vector3 legColor1, Vector3 legColor2, float scale = 1f, float inverse = 1, bool bend = true)
        {
            // prepare the hand
            Asset3d tip = new Asset3d();
            tip = P_makeLegs(center, legColor2, legColor1, 2 * scale);
            tip.rotate(tip._centerPosition, tip._euler[0], -90); // rotated 
            for (int i = 0; i < 3; i++) // normalized (reset euler)
            {
                tip._euler[i] = Vector3.Normalize(tip._euler[i]);
            }

            // make the hand a joint object
            Asset3d joint = new Asset3d();
            joint = joint.P_makeArmJoint(tip, center, new Vector3(25, 255, 160), 0.5f * scale); // fancy color
            _flageltips.addChild(joint); // add to this for easier access

            // make a flagel
            Asset3d flagel = new Asset3d();
            return P_makeFlagel(joint, center, legColor1, legColor2, scale, inverse, bend);
        }
        public Asset3d P_makeFlagel(Asset3d tip, Vector3 center, Vector3 legColor1, Vector3 legColor2, float scale = 1f, float inverse = 1, bool bend = true)
        {
            Asset3d flagel = new Asset3d();
            List<float> bendList = new List<float>();
            flagel = flagel.P_makeChain(tip, center, legColor1, legColor2, scale * 0.5f, scale * 0.5f, 36);
            if (bend)
            {
                bendList = flagel.A_setJointCurl(bendList, 90 * inverse, 12);
                bendList = flagel.A_setJointCurl(bendList, -90 * inverse, 12);
                bendList = flagel.A_setJointCurl(bendList, 90 * inverse, 12);
                bendList.Reverse();
                flagel.A_furl(flagel, bendList.Count - 1, bendList, 1);
                flagel.rotate(flagel._centerPosition, flagel._euler[1], -inverse * 45);

            }

            flagel.rotate(flagel._centerPosition, flagel._euler[1], 15 * inverse);
            return flagel;
        }
        public Asset3d P_makeHead(Vector3 center, Vector3 bodyColor, float scale = 0.5f)
        {
            Asset3d body = new Asset3d();
            Asset3d segments = new Asset3d();
            float height = 0.5f * scale; // why didn't i think of this sooner
            float tipHeight = 0.6f * scale;
            Vector3 gradient = getGradient(bodyColor, new Vector3(50, 0, 255), 6);
            // bottom tip
            segments.F_createPrism(center, 8, 0, 0.8f * scale, tipHeight);
            bodyColor += gradient;
            segments.setColor(bodyColor);
            segments.rotate(segments._centerPosition, segments._euler[0], -90);
            center.Y += tipHeight;
            body.addChild(segments);
            segments = new Asset3d();
            // main body

            segments.F_createPrism(center, 8, 0.8f * scale, scale, height);
            bodyColor += gradient;
            segments.setColor(bodyColor);
            segments.rotate(segments._centerPosition, segments._euler[0], -90);
            center.Y += height;
            body.addChild(segments);
            segments = new Asset3d();

            segments.F_createPrism(center, 8, scale, scale, height);
            bodyColor += gradient;
            segments.setColor(bodyColor);
            segments.rotate(segments._centerPosition, segments._euler[0], -90);
            center.Y += height;
            body.addChild(segments);
            segments = new Asset3d();

            // make eye here
            segments = P_makeEye(new Vector3(center.X, center.Y, center.Z + scale), 0.3f * scale);
            bodyColor += gradient;
            segments.rotate(segments._centerPosition, segments._euler[0], 90);
            body.addChild(segments);
            segments = new Asset3d();
            // resume segment

            segments.F_createPrism(center, 8, scale, scale, height);
            bodyColor += gradient;
            segments.setColor(bodyColor);
            segments.rotate(segments._centerPosition, segments._euler[0], -90);
            center.Y += height;
            body.addChild(segments);
            segments = new Asset3d();

            segments.F_createPrism(center, 8, scale, 0.8f * scale, height);
            bodyColor += gradient;
            segments.setColor(bodyColor);
            segments.rotate(segments._centerPosition, segments._euler[0], -90);
            center.Y += height;
            body.addChild(segments);
            segments = new Asset3d();

            // top tip
            segments.F_createPrism(center, 8, 0.8f * scale, 0, tipHeight);
            bodyColor += gradient;
            segments.setColor(bodyColor);
            segments.rotate(segments._centerPosition, segments._euler[0], -90);
            center.Y += tipHeight;
            body.addChild(segments);
            segments = new Asset3d();

            // crown
            //segments = segments.P_makeDonutRing(center,new Vector3(140, 0, 255), 0.3f);
            //body.addChild(segments);
            return body;
        }
        public Asset3d P_makeEntity(Vector3 center, Vector3 colorSeed1, Vector3 colorSeed2, float scale)
        {
            Asset3d entity = new Asset3d(center);
            _flagels = new Asset3d();
            // legs
            _legs = P_makeLegs(entity._centerPosition, colorSeed1, colorSeed2, 4 * scale);
            entity.addChild(_legs);

            // head
            _head = P_makeHead(entity._centerPosition, colorSeed1, scale);
            entity.addChild(_head);

            // the flagels
            Asset3d flagel = new Asset3d();
            flagel = P_makeCompleteFlagel(entity._centerPosition, new Vector3(25, 0, 160), new Vector3(25, 255, 160), 0.1f, 1);

            _flagels.addChild(flagel); // for ease of control
            entity.addChild(flagel);

            flagel = new Asset3d();
            flagel = P_makeCompleteFlagel(entity._centerPosition, new Vector3(25, 0, 160), new Vector3(25, 255, 160), 0.1f, -1);

            _flagels.addChild(flagel); // for ease of control
            entity.addChild(flagel);

            return entity;
        }
    }
}

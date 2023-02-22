using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LasterMove : MonoBehaviour
{
    public bool vertical = true;
    public bool horizontal = false;
    public float lasterSpeed = 3F;
    public float stopX = 20F;
    public float stopY = 66.5F;
    public GameObject follow;
    public GameObject LasterGameObject;

    private EGA_Laser EGA;
    private LineRenderer Laster;
    private bool isMove = true;
    private bool check102;
    private bool check204;

    #region (MoveDirection)
    private Vector2 fatherDirection;
    private Vector2 lasterDirection;
    private Vector2 sonDirection;
    #endregion

    #region (RaycastDirection)
    private Vector3 fatherRay;
    private Vector3 sonRay;
    #endregion

    private Vector4 Length = new Vector4(1, 1, 1, 1);

    private void Start()
    {
        Laster = LasterGameObject.GetComponent<LineRenderer>();
        EGA = LasterGameObject.GetComponent<EGA_Laser>();

        if (vertical)
        {
            fatherDirection = new Vector2(1,0);
            sonDirection = new Vector2(1, 0);
            lasterDirection = new Vector2(0,1);
            fatherRay = -transform.up;
            sonRay = transform.up;

            LasterGameObject.transform.rotation = Quaternion.Euler(new Vector3(90F, 90F, 0));
        }

        if (horizontal)
        {
            fatherDirection = new Vector2(0,1);
            sonDirection = new Vector2(0,1);
            lasterDirection = new Vector2(1,0);
            fatherRay = -transform.right;
            sonRay = transform.right;

            LasterGameObject.transform.rotation = Quaternion.Euler(new Vector3(180F, 90F, 0));
        }
    }

    private void Update()
    {
        if (isMove)
        {
            LasterGameObject.transform.Translate(lasterSpeed * Time.deltaTime * lasterDirection);
            follow.transform.Translate(lasterSpeed * Time.deltaTime * sonDirection);
            transform.Translate(lasterSpeed * Time.deltaTime * fatherDirection);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, fatherRay, EGA.MaxLength, LayerMask.GetMask("NormalPlatForm"));
            RaycastHit2D afterHit = Physics2D.Raycast(follow.transform.position, sonRay, EGA.MaxLength, LayerMask.GetMask("NormalPlatForm"));
            RaycastHit2D characterHit = Physics2D.Raycast(follow.transform.position, sonRay, EGA.MaxLength, LayerMask.GetMask("Player"));

            AdjustFollowPosition(hit);

            if (hit.collider != null)
            {
                Laster.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
                Laster.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));

                Debug.DrawLine(transform.position, hit.point, Color.red);
                Laster.transform.position = hit.point;

                Laster.SetPosition(0, hit.point);

                if (afterHit.collider != null)
                {
                    Debug.DrawLine(follow.transform.position, afterHit.point, Color.red);

                    if (characterHit.collider != null)
                    {
                        Debug.DrawLine(follow.transform.position, characterHit.point, Color.red);

                        RaycastCharacter(characterHit.collider);
                        Laster.SetPosition(1, characterHit.point);
                        EGA.HitEffect.transform.position = characterHit.point + -1F * EGA.HitOffset * characterHit.normal;
                    }
                    else
                    {
                        Laster.SetPosition(1, afterHit.point);
                        EGA.HitEffect.transform.position = afterHit.point + -1F * EGA.HitOffset * afterHit.normal;
                    }

                    foreach (var AllPs in EGA.Effects)
                    {
                        if (!AllPs.isPlaying) AllPs.Play();
                    }
                    Length[0] = EGA.MainTextureLength * (Vector3.Distance(transform.position, afterHit.point));
                    Length[2] = EGA.NoiseTextureLength * (Vector3.Distance(transform.position, afterHit.point));
                }
                if (Laster.enabled == false && EGA.LaserSaver == false)
                {
                    EGA.LaserSaver = true;
                    Laster.enabled = true;
                }
            }
        }
        else
        {
            RaycastHit2D characterHit = Physics2D.Raycast(follow.transform.position, sonRay, EGA.MaxLength, LayerMask.GetMask("Player"));
            if (characterHit.collider != null)
            {
                RaycastCharacter(characterHit.collider);
                Laster.SetPosition(1, characterHit.point);
                EGA.HitEffect.transform.position = characterHit.point + -1F * EGA.HitOffset * characterHit.normal;
            }
        }
    }

    private void AdjustFollowPosition(RaycastHit2D hit)
    {
        if (horizontal)
        {
            if (Mathf.Abs(hit.point.x - follow.transform.position.x) 
                >= Mathf.Abs(transform.position.x - follow.transform.position.x))
                return;

            if (transform.position.y >= stopY)
            {
                isMove = false;
            }
        }

        if (vertical)
        {
            if (transform.position.x >= stopX)
            {
                isMove = false;
            }
        }
    }

    private void RaycastCharacter(Collider2D _collision)
    {
        if (_collision.GetComponent<SkillSystem.ShieldDeployer>() != null && _collision.GetComponent<SkillSystem.ShieldDeployer>().shield != 0)
        {
            _collision.GetComponent<SkillSystem.ShieldDeployer>().BeHurt(10);
        }
        else
        {
            _collision.GetComponent<RealCharacterController>().Hurt(1, 10);
        }
    }

    private void OnEnable()
    {
        isMove = true;
    }

    //private void Update()
    //{
    //    LasterGameObject.transform.Translate(lasterSpeed * Time.deltaTime * lasterDirection);
    //    follow.transform.Translate(lasterSpeed * Time.deltaTime * sonDirection);
    //    transform.Translate(lasterSpeed * Time.deltaTime * fatherDirection);

    //    RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up);

    //    if (hit.collider != null)
    //    {
    //        Laster.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));
    //        Laster.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));

    //        Debug.DrawLine(transform.position, hit.point, Color.red);
    //        Laster.transform.position = hit.point;

    //        Laster.SetPosition(0, hit.point);

    //        RaycastHit2D afterHit = Physics2D.Raycast(new Vector2(hit.point.x, hit.point.y - 0.5F), -transform.up);

    //        if (afterHit.collider != null)
    //        {
    //            Laster.SetPosition(1, afterHit.point);
    //            EGA.HitEffect.transform.position = afterHit.point + afterHit.normal * EGA.HitOffset;
    //            foreach (var AllPs in EGA.Effects)
    //            {
    //                if (!AllPs.isPlaying) AllPs.Play();
    //            }
    //            Length[0] = EGA.MainTextureLength * (Vector3.Distance(transform.position, afterHit.point));
    //            Length[2] = EGA.NoiseTextureLength * (Vector3.Distance(transform.position, afterHit.point));
    //        }
    //        else
    //        {
    //            var EndPos = transform.position + transform.forward * EGA.MaxLength;
    //            Laster.SetPosition(1, EndPos);
    //            EGA.HitEffect.transform.position = EndPos;
    //            foreach (var AllPs in EGA.Hit)
    //            {
    //                if (AllPs.isPlaying) AllPs.Stop();
    //            }
    //            Length[0] = EGA.MainTextureLength * (Vector3.Distance(transform.position, EndPos));
    //            Length[2] = EGA.NoiseTextureLength * (Vector3.Distance(transform.position, EndPos));
    //        }
    //        if (Laster.enabled == false && EGA.LaserSaver == false)
    //        {
    //            EGA.LaserSaver = true;
    //            Laster.enabled = true;
    //        }
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarcissSwordStage1 : MonoBehaviour
{
    public float MoveSpeed = 8f;
    public float AliveSecond = 10f;
    public float TimeScale { get; set; } = 1f;
    float m_timer = -1;
    // Start is called before the first frame update
    void Start()
    {
        EventCenter.Instance.AddEventListener<SlowTimeArgs>(Consts.E_BUFF_TimeSlow, tiemSlowEvent);
        EventCenter.Instance.AddEventListener(Consts.E_BUFF_TimeNormal, timeNormalEvent);
    }

    void tiemSlowEvent(SlowTimeArgs args)
    {
        TimeScale = args.magnification;
    }

    void timeNormalEvent()
    {
        TimeScale = 1;
    }

    void OnEnable()
    {
        m_timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_timer>=0)
            m_timer += Time.deltaTime*TimeScale;
        if(m_timer>=AliveSecond)
        {
            m_timer = -1;
            Game.Instance.ObjectPool.RecycleObject(gameObject);
        }
            
    }
    private void FixedUpdate()
    {
        transform.Translate((transform.rotation.y == 0 ? -transform.right : transform.right) * MoveSpeed * Time.fixedDeltaTime * TimeScale);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.transform.tag == "Player")
    //    {
    //        int faceTo = (transform.position.x) > (collision.transform.position.x) ? -1 : 1;
    //        collision.transform.GetComponent<RealCharacterController>().Hurt(faceTo, 1);
    //        if (collision.gameObject.layer == 14)
    //            Game.Instance.ObjectPool.RecycleObject(gameObject);
    //        else
    //            collision.gameObject.GetComponent<RealCharacterController>().CheckScratch();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (collision.gameObject.layer == 14)
                collision.gameObject.GetComponent<RealCharacterController>().CheckScratch();
            else
                Game.Instance.ObjectPool.RecycleObject(gameObject);
            int faceTo = (transform.position.x) > (collision.transform.position.x) ? -1 : 1;
            RealCharacterController rcc = collision.transform.GetComponent<RealCharacterController>();
            if (rcc)
                rcc.Hurt(faceTo, 1);
        }
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<SlowTimeArgs>(Consts.E_BUFF_TimeSlow, tiemSlowEvent);
        EventCenter.Instance.RemoveEventListener(Consts.E_BUFF_TimeNormal, timeNormalEvent);
    }
}

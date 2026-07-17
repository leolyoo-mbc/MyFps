using System.Collections;
using UnityEngine;

namespace Sample
{
    public class EnemyTest : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;

        public float health = 10f;
        private bool isDetah = false;

        //애니메이터 파라미터
        private const string Hurt = "Hurt";
        private const string Death = "Death";

        //디졸브 효과
        public Renderer renderer;

        private Material originMaterial;
        public Material dessolveMaterial;

        public GameObject vfxObejct;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            //초기화
            originMaterial = renderer.material;
            if (vfxObejct != null) vfxObejct.SetActive(false);

            StartCoroutine(SpawnEnemy());


        }

        private void Update()
        {
            //
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(5f);
            }*/
        }
        #endregion

        #region Custom Method
        public void TakeDamage(float damage)
        {
            health -= damage;


            if (health <= 0f && isDetah == false)
            {
                Die();
            }
            else
            {
                //애니메이션 처리
                animator.SetTrigger(Hurt);
            }

        }

        private void Die()
        {
            isDetah = true;

            //애니메이션 연출
            animator.SetTrigger(Death);

            //vfx 연출
            StartCoroutine(DestroyEnemy());
        }

        //생성
        IEnumerator SpawnEnemy()
        {
            renderer.material = dessolveMaterial;
            renderer.material.SetFloat("_SplitValue", 0f);

            if (vfxObejct != null)
            {
                if (vfxObejct.activeSelf) vfxObejct.SetActive(false);

                vfxObejct.SetActive(true);
            }

            float t = 0f;

            while (t < 1.5f)
            {
                t += Time.deltaTime;
                float value = t / 1.5f;
                renderer.material.SetFloat("_SplitValue", value);

                yield return null;
            }

            yield return new WaitForSeconds(2f);

            // 테스트용 데미지 주기
            TakeDamage(10f);
        }


        //소멸
        IEnumerator DestroyEnemy()
        {
            yield return new WaitForSeconds(0.5f);

            renderer.material = dessolveMaterial;
            renderer.material.SetFloat("_SplitValue", 1f);

            if (vfxObejct != null)
            {
                if (vfxObejct.activeSelf) vfxObejct.SetActive(false);

                vfxObejct.SetActive(true);
            }
            float t = 0f;

            while (t < 1.5f)
            {
                t += Time.deltaTime;
                float value = t / 1.5f;
                renderer.material.SetFloat("_SplitValue", 1f - value);

                yield return null;
            }

            yield return new WaitForSeconds(2f);

            Destroy(gameObject);
            //renderer.material = originMaterial;
        }
        #endregion
    }
}
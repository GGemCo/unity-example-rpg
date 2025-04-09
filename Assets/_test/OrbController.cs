using GGemCo.Scripts;
using UnityEngine;

// Spine API 사용

namespace _test
{
    public class OrbController : MonoBehaviour
    {
        public enum OrbitType { Circle, Ellipse, VerticalBounce, HorizontalBounce, Random }
        public OrbitType orbitType = OrbitType.Circle; // 기본 궤도 유형
        
        public float orbitRadius = 1.5f;  // 기본 반지름 (원형)
        public float orbitSpeed = 2f;  // 회전 속도
        public float ellipseWidth = 2f;  // 타원 궤도 가로축
        public float ellipseHeight = 1f; // 타원 궤도 세로축
        public float bounceHeight = 1f;  // 상하 진동 높이
        public float bounceWidth = 1.5f; // 좌우 진동 너비
        public float randomFactor = 0.5f; // 랜덤 궤도 변동성
        
        public SpriteRenderer spriteRenderer;
        private MeshRenderer playerMeshRenderer; // Spine MeshRenderer
        private GameObject player;  // 플레이어 Transform

        private float angle; // 현재 회전 각도
        private float randomOffset;

        void Start()
        {
            randomOffset = Random.Range(0f, 2f * Mathf.PI); // 랜덤 패턴 초기화
        }

        void Update()
        {
            if (SceneGame.Instance == null || SceneGame.Instance.player == null) return;
            if (player == null)
            {
                player = SceneGame.Instance.player;
                playerMeshRenderer = player.GetComponent<MeshRenderer>(); // 플레이어의 MeshRenderer 가져오기
            }
            
            angle += orbitSpeed * Time.deltaTime; // 회전각 증가

            Vector3 newPosition = transform.position;

            switch (orbitType)
            {
                case OrbitType.Circle:
                    newPosition = GetCircularOrbit();
                    break;
                case OrbitType.Ellipse:
                    newPosition = GetEllipticalOrbit();
                    break;
                case OrbitType.VerticalBounce:
                    newPosition = GetVerticalBounce();
                    break;
                case OrbitType.HorizontalBounce:
                    newPosition = GetHorizontalBounce();
                    break;
                case OrbitType.Random:
                    newPosition = GetRandomOrbit();
                    break;
            }

            transform.position = newPosition;

            // 플레이어의 Y좌표를 기준으로 Sorting Order 조정
            if (transform.position.y > player.transform.position.y)
            {
                spriteRenderer.sortingOrder = playerMeshRenderer.sortingOrder - 1; // 뒤쪽
            }
            else
            {
                spriteRenderer.sortingOrder = playerMeshRenderer.sortingOrder + 1; // 앞쪽
            }
        }
        // 🟢 1) 원형 궤도
        private Vector3 GetCircularOrbit()
        {
            float x = player.transform.position.x + Mathf.Cos(angle) * orbitRadius;
            float y = player.transform.position.y + Mathf.Sin(angle) * orbitRadius;
            return new Vector3(x, y, 0);
        }

        // 🔵 2) 타원 궤도 (가로/세로 비율 다름)
        private Vector3 GetEllipticalOrbit()
        {
            float x = player.transform.position.x + Mathf.Cos(angle) * ellipseWidth;
            float y = player.transform.position.y + Mathf.Sin(angle) * ellipseHeight;
            return new Vector3(x, y, 0);
        }

        // 🔴 3) 상하 진동 (플레이어 기준 수직으로 움직임)
        private Vector3 GetVerticalBounce()
        {
            float x = player.transform.position.x;
            float y = player.transform.position.y + Mathf.Sin(angle) * bounceHeight;
            return new Vector3(x, y, 0);
        }

        // 🟡 4) 좌우 진동 (플레이어 기준 수평으로 움직임)
        private Vector3 GetHorizontalBounce()
        {
            float x = player.transform.position.x + Mathf.Sin(angle) * bounceWidth;
            float y = player.transform.position.y;
            return new Vector3(x, y, 0);
        }

        // 🟣 5) 랜덤 궤도 (랜덤한 패턴으로 움직임)
        private Vector3 GetRandomOrbit()
        {
            float x = player.transform.position.x + Mathf.Cos(angle + randomOffset) * (orbitRadius + Random.Range(-randomFactor, randomFactor));
            float y = player.transform.position.y + Mathf.Sin(angle + randomOffset) * (orbitRadius + Random.Range(-randomFactor, randomFactor));
            return new Vector3(x, y, 0);
        }
    }
}
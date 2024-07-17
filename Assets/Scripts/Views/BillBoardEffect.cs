using UnityEngine;

namespace Views
{
    public class BillBoardEffect : MonoBehaviour
    {
        private void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
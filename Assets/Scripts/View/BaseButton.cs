using UnityEngine;

namespace View
{
    public class BaseButton : MonoBehaviour
    {
        [SerializeField] private Base headQuarter;
    
        public void OnMouseDown()
        {
            BaseView.Get().Show(headQuarter.GetId());
        }
    }
}

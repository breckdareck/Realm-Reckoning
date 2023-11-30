using UnityEngine;
using UnityEngine.UI;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Misc
{
    public class Scroller : MonoBehaviour
    {
        [SerializeField] private RawImage _img;
        [SerializeField] private float _x, _y;

        private void Update()
        {
            _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
        }
    }
}
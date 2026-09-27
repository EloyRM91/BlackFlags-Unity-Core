using UnityEngine;
using UnityEngine.UI;

namespace UI.Files
{
    public class SavedFileError : MonoBehaviour
    {
        [SerializeField] private Text _TEXT_Error;

        public void updateErrorCause(string msg)
        {
            _TEXT_Error.text = msg;
        }
    }
}


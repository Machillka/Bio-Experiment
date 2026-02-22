using UnityEngine;
namespace PCR.Step1
{
    public class DatabaseManager : StepBase
    {
        private static DatabaseManager _instance;
        public static DatabaseManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<DatabaseManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("SimpleSingleton");
                        _instance = obj.AddComponent<DatabaseManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

        public string TargetSequence = "";

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public override void OnEnter()
        {
            OnExit();
        }

        public override void OnExit()
        {
            Data.TargetDnaSequence = "ATCGATCGATCG";
            PCRManager.Instance.NextStep();
        }

    }

}

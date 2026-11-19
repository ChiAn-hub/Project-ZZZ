using UnityEngine;
using UnityEngine.UI;

public class EnemyHp : MonoBehaviour
{
    [SerializeField]
    private Image hpImage;

    private EnemyManage enemy;
    private EnemyModel enemyModel;
    private Camera mainCamera;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyManage>();
        mainCamera = Camera.main;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemy == null)
        {
            Debug.LogError("EnemyHp没有找到EnemyManage");
            return;
        }

        if (hpImage == null)
        {
            Debug.LogError("EnemyHp没有绑定血条Image");
            return;
        }
        enemyModel = enemy.EnemyModel;
        enemyModel.HpChanged += RefreshHp;//监听血量变化

        RefreshHp(enemyModel.CurHp, enemyModel.MaxHp);//显示初始血量
    }

    void OnEnable()//再次被启用时
    {
        if (enemyModel != null)
        {
            RefreshHp(enemyModel.CurHp, enemyModel.MaxHp);
        }
    }

    void OnDestroy()
    {
        if (enemyModel != null)
        {
            enemyModel.HpChanged -= RefreshHp;//取消监听
        }
    }

    void LateUpdate()//血条始终朝向摄像机
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }
        transform.rotation = mainCamera.transform.rotation;
    }

    private void RefreshHp(int curHp, int maxHp)//刷新血条
    {
        if (hpImage == null || maxHp <= 0)
        {
            return;
        }

        hpImage.fillAmount = (float)curHp / maxHp;
    }
}

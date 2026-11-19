using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveInput;//记录玩家输入的朝向
    private bool isEvadePressed;
    private bool isAttackPressed;
    private bool isSkillPressed;
    private bool isExSkillPressed;
    public bool IsExSkillPressed
    {
        get
        {
            return isExSkillPressed;
        }
    }
    public bool IsSkillPressed
    {
        get
        {
            return isSkillPressed;
        }
    }
    public bool IsAttackPressed//获取玩家是否按下了攻击键
    {
        get
        {
            return isAttackPressed;
        }
    }
    public bool IsEvadePressed//获取玩家是否按下了闪避键
    {
        get
        {
            return isEvadePressed;
        }
    }
    public Vector2 MoveInput//获取玩家输入的朝向
    {
        get
        {
            return moveInput;
        }
    }
    public bool HasMoveInput///判断玩家是否输入
    {
        get
        {
            return moveInput.sqrMagnitude > 0.01f;
        }
    }
    public static bool CanControl { get; private set; } = true;//玩家是否可控制
    // Update is called once per frame
    void Update()
    {
        if (!CanControl)//玩家不可控制
        {
            moveInput = Vector2.zero;//输入归零
            isEvadePressed = false;//禁止闪避
            isAttackPressed = false;
            isSkillPressed = false;
            isExSkillPressed = false;
            return;
        }

        ReadMoveInput();
        ReadEvadeInput();
        ReadAttackInput();
        ReadSkillInput();
        ReadExSkillInput();
    }
    private void ReadMoveInput()//读取玩家输入
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
    }
    private void ReadEvadeInput()//读取玩家闪避输入
    {
        isEvadePressed = Input.GetMouseButtonDown(1);
    }
    private void ReadAttackInput()//读取玩家攻击输入
    {
        isAttackPressed = Input.GetMouseButtonDown(0);
    }
    private void ReadSkillInput()//读取技能输入
    {
        isSkillPressed = Input.GetKeyDown(KeyCode.E);
    }
    private void ReadExSkillInput()//读取技能输入
    {
        isExSkillPressed = Input.GetKeyDown(KeyCode.Q);
    }
    public static void SetCanControl(bool canControl)//接口,设置玩家是否可控制
    {
        CanControl = canControl;
    }
}

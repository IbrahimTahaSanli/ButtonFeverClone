using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyController : MonoBehaviour
{
    [HideInInspector] public static MoneyController instance;

    [SerializeField] private int ClickIntervalSecond;
    [SerializeField] private TMPro.TMP_Text moneyText;
    [SerializeField] private MoneyPlusController MoneyPlus;
    [SerializeField] private int BlockPrice;
    [SerializeField] private TMPro.TMP_Text buyButtonText;

    [HideInInspector] private int _money;
    [HideInInspector] public int Money
    {
        get { return _money; }
        set {
            _money = value;
            moneyText.text = value.ToString();
        }
    }

    private Coroutine clickCoroutine;



    public MoneyController()
    {
        if (instance != null)
            return;

        instance = this;

        Money = 100;
    }

    // Start is called before the first frame update
    void Start()
    {
        clickCoroutine = StartCoroutine(ClickForSecond());
        buyButtonText.text = "Get Block(" + BlockPrice.ToString() + ")";
    }

    private IEnumerator ClickForSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(ClickIntervalSecond);
            Click();
        }
    } 

    public void Click()
    {
        int MoneyForClick = 0;

        foreach (Block block in GridController.instance.ActiveBlocks)
            MoneyForClick += (int)block.type;

        Money += (int)(MoneyForClick * GridController.instance.TotalRowMultiplier);

        if (MoneyForClick > 0)
            MoneyPlus.ShowMoney(MoneyForClick.ToString());
    }

    public void BuyBlock()
    {
        if (Money < BlockPrice)
            return;

        if (BlockController.instance.IsFree() == -1)
            return;

        Money -= BlockPrice;
        BlockController.instance.GetNewBlock();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    [HideInInspector] private GameObject currentBlock;
    [HideInInspector] private Vector2 GrabbedPosition;

    [HideInInspector] private Vector3 LastMouseVector;

    [HideInInspector] private Vector2? TakenFrom;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, int.MaxValue, LayerMask.GetMask("Block")))
            {
                currentBlock = hit.collider.transform.parent.gameObject;

                Block tmpBlock = currentBlock.GetComponent<Block>();
                if (tmpBlock.IsInGrid)
                {
                    TakenFrom = tmpBlock.PosInGrid;
                    GridController.instance.RemoveFromGrid(tmpBlock);
                }
                else
                {
                    BlockController.instance.RemoveBlock(tmpBlock);
                }

                GrabbedPosition = new Vector2(int.Parse(hit.collider.name.Split('-')[1]), int.Parse(hit.collider.name.Split('-')[0]));
                hit.transform.position.Set(hit.transform.position.x, hit.transform.position.y, -4);
            }
            LastMouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            MoneyController.instance.Click();
        }

        else if (Input.GetMouseButton(0)) 
        {
            if (currentBlock == null)
                return;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - LastMouseVector;
            currentBlock.transform.position += pos;
            LastMouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        else if (Input.GetMouseButtonUp(0))
        {
            if (currentBlock == null)
                return;


            Block block = currentBlock.GetComponent<Block>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            currentBlock = null;
            //InGrid
            if (Physics.Raycast(ray, out hit, int.MaxValue, LayerMask.GetMask("Grid")))
            {
                Vector2 gridLoc = GridController.instance.LocalSpaceToGridCell(hit.collider.transform.InverseTransformPoint(hit.point));
                gridLoc = gridLoc - GrabbedPosition;
                
                if (GridController.instance.CheckIsEmpty(block, gridLoc))
                {
                    GridController.instance.SetBlock(block, gridLoc);
                    return;
                }
            }

            //Garbage
            if (Physics.Raycast(ray, out hit, int.MaxValue, LayerMask.GetMask("Garbage")))
            {
                Destroy(block.gameObject);
                return;
            }

            //Upgrade
            RaycastHit[] hits = Physics.RaycastAll(ray, int.MaxValue, LayerMask.GetMask("Block"));
            if (hits.Length > 1)
            {
                foreach (RaycastHit objHit in hits)
                    if (objHit.collider.transform.parent != block.transform)
                    {
                        Block otherBlock = objHit.collider.transform.parent.GetComponent<Block>();
                        if (otherBlock.type == Block.Blocks.Block8)
                            break;
                        BlockController.instance.UpgradeBlock(otherBlock, block);
                        return;
                    }
            }

            //Taken From Grid But Cant Placed
            if (TakenFrom != null)
            {
                GridController.instance.SetBlock(block, (Vector2)TakenFrom);
                TakenFrom = null;
                return;
            }

            //Havent Placed
            if (BlockController.instance.IsFree() >= 0)
            {
                BlockController.instance.PutBlock(block);
                return;
            }

            Destroy(block.gameObject);
        }
    }

    private void SetLayerRecursivly(GameObject obj, LayerMask mask)
    {
        foreach (Transform t in obj.transform)
            SetLayerRecursivly(t.gameObject, mask);

        obj.layer = mask;
    }
}

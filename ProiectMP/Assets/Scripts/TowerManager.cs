using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

    private TowerBtn towerBtnPressed;
    private SpriteRenderer spriteRenderer;
    private List<Tower> towerList = new List<Tower>();
    private List<Collider2D> buildList = new List<Collider2D>();
    private Collider2D buildTile;
    private List<Projectile> projList = new List<Projectile>();
    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.pause == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider.tag == "BuildSide")
                {
                    buildTile = hit.collider;
                    buildTile.tag = "BuildSideFull";
                    hit.collider.tag = "BuildSideFull";
                    registerBuildSite(buildTile);
                    PlaceTower(hit);
                }
            }
            if (spriteRenderer.enabled)
            {
                FollowMouse();
            }
        }
    }

    public void addProjectile(Projectile p) {
        projList.Add(p);
    }

    public void destroyAllProjectiles()
    {
        foreach (Projectile t in projList)
        {
            Destroy(t.gameObject);
        }
        projList.Clear();
    }
    public void registerBuildSite(Collider2D buildTag)
    {
        buildList.Add(buildTag);
    }

    public void registerTower(Tower t)
    {
        towerList.Add(t);
    }

    public void renameTagsBuildSite()
    {
        foreach(Collider2D buildTag in buildList)
        {
            buildTag.tag = "BuildSide";
        }
        buildList.Clear();
    }

    public void destroyAllTowers()
    {
        foreach(Tower t in towerList)
        {
            Destroy(t.gameObject);
        }
        towerList.Clear();
    }
    public void PlaceTower(RaycastHit2D hit)
    {
        if (GameManager.Instance.pause == false)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
            {
                Tower newTower = Instantiate(towerBtnPressed.TowerObject);
                newTower.transform.position = hit.transform.position;
                buyTower(towerBtnPressed.TowerPrice);
                registerTower(newTower);
                towerBtnPressed = null;
                GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuild);
                DisableDragSprite();
            }
        }
    }

    public void SelectedTower(TowerBtn towerSelected) {
        if (GameManager.Instance.pause == false)
        {
            if (GameManager.Instance.TotalMoney >= towerSelected.TowerPrice)
            {
                towerBtnPressed = towerSelected;
                EnableDragSprite(towerBtnPressed.DragSprite);
            }
        }
    }

    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void EnableDragSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.enabled = true;
    }

    public void DisableDragSprite()
    {
        spriteRenderer.sprite = null;
        spriteRenderer.enabled = false;
    }

    public void buyTower(int price) {
        GameManager.Instance.subtractMoney(price);
    }
    public TowerBtn TowerBtnPressed
    {
        get
        {
            return towerBtnPressed;
        }
        set
        {
            towerBtnPressed = value;
        }
    }
}

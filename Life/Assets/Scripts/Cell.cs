using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Cell : MonoBehaviour
{
    public enum CELLTYPE { Good, Bad, Neutral };
    //public enum LMH { none, light, medium, heavy };
    //public enum KINDSOFAURA { none, slow, fast }

    public static List<Cell> AllCells = new List<Cell>();
    public static int GoodCellsCount = 0;
    public static int BadCellsCount = 0;
    public static int NeutralCellsCount = 0;

    public CELLTYPE cellType = CELLTYPE.Neutral;

    [Header("--- Leben: ---------")]
    public bool usePhotosynthesis = true;  
	[Range(1, 3)]
	public float healRate = 1.0f;
    public bool consumeEnergy = true;
    [Range(0, 2)]
    public float energyConsumption = 0.75f;
    public bool canAbsorbATP = false;
    bool applyPhotosynthesis = true;
    public float lifepoints = 100;
    //bool applyHealRay = false;
    public float maximalLifePoints = 100;

    //public LMH tankyness = LMH.light;
    //float armor = 0.1f;

    [Header("--- Split Cell: ---------")]
    //  public LMH splitThreshhold = LMH.light;
    public bool cellSplits = true;
    [Range(0, 1)]
    public float splitRatio = 0.5f;
    public bool mutatesAtSplit = true;

    [Header("--- Damage: ---------")]
    public bool canDrain = false;
    public bool hasSpikes = false;
	//public bool hasAura = false;
    public bool isPoisonous = false;

    //public KINDSOFAURA aura = KINDSOFAURA.none;
    //public LMH shotFrequency = LMH.none;
    //public float shootDamage = 0;
    //public LMH powerShot = LMH.none;

    //[Header("--- Grafik: ---------")]
    //public Sprite sprite;

    [Header("Spawns When Dies")]
    public bool dropLoot = false;
    public GameObject loot;
    public GameObject poison;

    private bool wasAddedToTheTotalList = false;

    //==========================================================================



    // Use this for initialization
    void Start()
    {
        UpdateCellList();
        StartCoroutine(SlowUpdate());
        UpdateCellSize();
        SetupWeapons();
        ApplyCellType();
    }

    private void UpdateCellList()
    {

        if (!wasAddedToTheTotalList)
        {
            AllCells.Add(this);
            wasAddedToTheTotalList = true;
        }

        /// Count through all cells
        GoodCellsCount = 0;
        NeutralCellsCount = 0;
        BadCellsCount = 0;
        foreach (Cell c in AllCells)
        {
            if (c.cellType == CELLTYPE.Bad)
                BadCellsCount++;

            if (c.cellType == CELLTYPE.Good)
                GoodCellsCount++;

            if (c.cellType == CELLTYPE.Neutral)
                NeutralCellsCount++;
        }
    }

    void OnDestroy()
    {
        AllCells.Remove(this);
        UpdateCellList();

        StopAllCoroutines();
    }

    /// <summary>
    /// Use this when Cells get some mutation during their lifetime
    /// </summary>
    public void Restart()
    {
        StopAllCoroutines();
        Start();
    }


    internal bool HasGene(DNA_MutationPickup.MUTATION gene)
    {
        switch(gene)
        {
                case DNA_MutationPickup.MUTATION.None:
                return true;
            
                case DNA_MutationPickup.MUTATION.CanUseATPOff:
                    return !canAbsorbATP;            

                case DNA_MutationPickup.MUTATION.CanUseATPOn:
                return canAbsorbATP;            

                case DNA_MutationPickup.MUTATION.DrainOff:
                return !canDrain;            

                case DNA_MutationPickup.MUTATION.DrainOn:
                return canDrain;            

                case DNA_MutationPickup.MUTATION.Good:
                return cellType == Cell.CELLTYPE.Good;
            
                case DNA_MutationPickup.MUTATION.Bad:
                return cellType == Cell.CELLTYPE.Bad;

                case DNA_MutationPickup.MUTATION.Neutral:
                return cellType == Cell.CELLTYPE.Neutral;

                case DNA_MutationPickup.MUTATION.SpikesOff:
                return !hasSpikes;

                case DNA_MutationPickup.MUTATION.SpikesOn:
                return hasSpikes;

                case DNA_MutationPickup.MUTATION.UsePhotoSynthesisOff:
                return !usePhotosynthesis;

                case DNA_MutationPickup.MUTATION.UsePhotoSynthesisOn:
                return usePhotosynthesis;

                case DNA_MutationPickup.MUTATION.CanSplitOn:
                return cellSplits;

                case DNA_MutationPickup.MUTATION.CanSplitOff:
                return !cellSplits;

                case DNA_MutationPickup.MUTATION.IsPoisonousOn:
                return isPoisonous;

            case DNA_MutationPickup.MUTATION.IsPoisonousOff:
                return !isPoisonous;

            case DNA_MutationPickup.MUTATION.DropLootOn:
                return dropLoot;

            case DNA_MutationPickup.MUTATION.DropLootOff:
                return !dropLoot;
        }

        return false;
    }



    private void SetupWeapons()
    {
        try
        {
            //Spikes:
            transform.FindChild("CellSpikes").gameObject.SetActive(hasSpikes);

            //Suckers for Drain:
            transform.FindChild("CellSuckers").gameObject.SetActive(canDrain);
        }
        catch (System.Exception e) { }

    }

    private void ApplyCellType()
    {
        /// Set Color:
        Color cellColor = Color.blue;

        switch (cellType)
        {
            case CELLTYPE.Bad: cellColor = Color.red; break;
            case CELLTYPE.Good: cellColor = Color.green; break;
            case CELLTYPE.Neutral: cellColor = new Color(0.3f,0.5f,1);break;// Color.white; break;
        }
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.material.color = cellColor;
        }

        gameObject.tag = cellType.ToString();
    }


    /// <summary>
    /// Slow Update is applied once per second
    /// </summary>
    /// <returns></returns>
    IEnumerator SlowUpdate()
    {
        float timeInterval = 0.5f;

        // Wait for a random time to simulate more realistic growth
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, timeInterval));


        while (true)
        {

            if (usePhotosynthesis && applyPhotosynthesis)
            {
                ApplyPhotosynthesis(timeInterval);
            }

            /// Reduce Energy of the Cell => she allways needs more nutrients
            ApplyEnergyConsumption(timeInterval);

            yield return new WaitForSeconds(timeInterval);
        }
    }

    private void ApplyEnergyConsumption(float timeInterval)
    {
        if (consumeEnergy)
        {
            AddLifepoints(energyConsumption * -2 * (1 / timeInterval));
        }
    }

    void ApplyPhotosynthesis(float timeInterval)
    {
        AddLifepoints((3 / timeInterval) * Level.GetLightAtPosition(transform.position) * healRate);
    }

    public void ApplyHealRay(float power, float timeInterval)
    {
        AddLifepoints(power * timeInterval);
    }

    public void ApplyDeathRay(float power, float timeInterval)
    {
        AddLifepoints(-power * timeInterval);
    }

    void AddLifepoints(float amount)
    {

        lifepoints = Mathf.Clamp(lifepoints + amount, 0, maximalLifePoints);
        CheckForDeath();

        if (cellSplits)
        {
            CheckForSplit();
        }
        UpdateCellSize();

    }

    private void UpdateCellSize()
    {
        /// Size is relative to the Lifepoints
        transform.localScale = Vector3.one * Mathf.Clamp((lifepoints / 100/*maximalLifePoints*/), 0.2f, 200);
    }

    /// <summary>
    /// Check if cell is big enough to split;
    /// </summary>
    private void CheckForSplit()
    {

        if (lifepoints >= maximalLifePoints)
        {
            SplitCell();
        }
    }

    private void CheckForDeath()
    {
        if (lifepoints <= 0)
            Die();

    }

    private void Die()
    {
        //spawn Energy which can be absorbed by others
        if (dropLoot)
        {
            if (isPoisonous)
            {
                Instantiate(poison, transform.position, Quaternion.identity);
            }
            else if (loot != null)
            {
                Instantiate(loot, transform.position, Quaternion.identity);
            }
        }

        //TODO: cool animation
        Destroy(this.gameObject);
    }

    /// <summary>
    /// - This wird zerstört, spawnt 2 identische klone und teilt Leben gemäß Splitratio
    /// - wird aufgerufen wenn lifepoints > threshhold
    /// 
    /// </summary>
    void SplitCell()
    {

        // Falls wir gerade gedraggt werden, soll sich die zelle nicht teilen können.
        if (transform.parent != null)
        {
            if (transform.parent.GetComponent<PlayerGrabber2>())
            {
                return;
            }
        }

        Cell child = Instantiate(this.gameObject).GetComponent<Cell>();
        child.gameObject.name = "Cell";

       // child.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        child.lifepoints = lifepoints * splitRatio;
        lifepoints = lifepoints * (1 - splitRatio);

        child.ApplyMutations();

        child.transform.position = child.transform.position + new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), 0).normalized * 0.25f;
        child.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
    }

    /// <summary>
    /// Mutations cause cells to change from one generation to another.
    /// Most important mutation is the development of good and bad cells out of neutral ones
    /// </summary>
    private void ApplyMutations()
    {
        if (!mutatesAtSplit)
            return;

        ///Cell type: Neutral can become good or bad
        if (cellType == CELLTYPE.Neutral)
        {
            int rand = UnityEngine.Random.Range(0, 100);


            if (rand < 20)
                cellType = CELLTYPE.Good;
            if (rand > 75)
                cellType = CELLTYPE.Bad;
        }
        Player.player.CellDivisionSound(gameObject.transform.position);
    }

    /// <summary>
    /// Tries to absorb energy from hit cell
    /// </summary>
    public void UseDrain(GameObject hit)
    {
        if (!canDrain)
            return;

        if (cellType == CELLTYPE.Neutral)
            return;

        Cell otherCell = hit.GetComponent<Cell>();


        if (otherCell != null)
        {
            // No Friendly Drain!
            if (cellType == otherCell.cellType)
                return;

            // Spikes are better than Drain
            if (otherCell.hasSpikes)
                return;

            otherCell.ApplyDrainDamage(this);
        }

    }

    /// <summary>
    /// Takes Lifepoints from this and gives them to cell
    /// </summary>
    /// <param name="othercell"></param>
    private void ApplyDrainDamage(Cell othercell)
    {
        //  float drainAmount = Mathf.Clamp(othercell.transform.localScale.magnitude - othercell.transform.localScale.magnitude, 0.1f, 1);

        float drainAmount = othercell.lifepoints * 0.25f;

        //Debug.Log("I am beeing drained! "+ drainAmount + " type: "+cellType.ToString());
        othercell.AddLifepoints(drainAmount);
        AddLifepoints(-drainAmount);
    }

    void ShootCircle()
    { }

    void UseSpikes(GameObject hit)
    {
        if (!hasSpikes)
            return;

        Cell otherCell = hit.GetComponent<Cell>();
        if (otherCell != null)
        {
            otherCell.ApplySpikesDamage();

        }
    }

    public void ApplySpikesDamage()
    {
        ///imune if I have spikes too:
        if (hasSpikes)
            return;

        ///Spikes should take 25% of the total lives:
        float amountToTake = 0.25f * maximalLifePoints;
        AddLifepoints(-amountToTake);

    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        UseSpikes(hit.gameObject);
        UseDrain(hit.gameObject);

        GetDamageFromPlayerSpikes(hit.gameObject);

        ApplyProjectile(hit.gameObject);

    }

    private void GetDamageFromPlayerSpikes(GameObject g)
    {
        Player p = g.GetComponent<Player>();
        if(p)
        {
            if(p.isUsingSpikes())
            {
                ApplySpikesDamage();
            }
        }
    }

    private void ApplyProjectile(GameObject projectileObject)
    {
        Projectile proj = projectileObject.GetComponent<Projectile>();
        if(proj!=null)
        {
            if (proj.projectileType == Projectile.ProjectileType.enemyDamage || proj.projectileType == Projectile.ProjectileType.friendlyDamage)
            {
                AddLifepoints(-10);
            }

            // LifeShot
            if (proj.projectileType == Projectile.ProjectileType.enemyHeal || proj.projectileType == Projectile.ProjectileType.friendlyHeal)
            {
                AddLifepoints(+10);
            }

            Destroy(proj.gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        CollectPickup(hit);
    }

    private void CollectPickup(Collider2D hit)
    {
        // ATP is used as Enegery = lives
        if (hit.tag == "Pickup_ATP" && canAbsorbATP)
        {
            AddLifepoints(10);
            Destroy(hit.gameObject);
        }

        if (hit.tag == "Pickup_PATP" && canAbsorbATP && !isPoisonous)
        {
            //Poisonous ATP kills the cell
            float amountToTake = maximalLifePoints;
            AddLifepoints(-amountToTake);
            Destroy(hit.gameObject);
        }

        // DNA causes mutations to a cell
        if (hit.tag == "Pickup_DNA")
        {
            Debug.Log("I HIT DNA!");
            hit.gameObject.GetComponent<DNA_MutationPickup>().ApplyMutationsTo(this);
        }
    }
}

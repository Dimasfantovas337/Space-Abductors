using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Skills : MonoBehaviour
{
    public static Skills Instance;
    [HideInInspector] public List<string> SkillList = new List<string>();
    public int SkillPoints = 0;
    [SerializeField] private TextMeshProUGUI _skillPointMenuText;
    [SerializeField] private Weapon _playerWeapon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _skillPointMenuText.text = SkillPoints + " skill points";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && GameManager.Instance.CurrentState == GameManager.State.Playing)
        {
            OpenSkillpointMenu();
        }
    }

    public void BuySkill(SkillParameter parameter)
    {
        if (SkillPoints < parameter.Price) 
            return;
        parameter.IsBought = true;
        SkillList.Add(parameter.SkillName);
        AddSkillpoints(-parameter.Price);
        RefreshSkills();
    }

    public void OpenSkillpointMenu()
    {
        Time.timeScale = 0;
        GameManager.Instance.SetState(GameManager.State.Paused);
        UIManager.Instance.SkillpointMenu.SetActive(true);
    }

    public void AddSkillpoints(int pointsToAdd)
    {
        SkillPoints += pointsToAdd;
        _skillPointMenuText.text = SkillPoints + " skill points";
    }

    public void RefreshSkills()
    {
        foreach (string skill in SkillList)
        {
            switch (skill) // Add a string here for every new weapon skill
            {
                case "Shotgun":
                    _playerWeapon.CurrentWeaponAttack = _playerWeapon.ShootLikeShootgun;
                    _playerWeapon.Railgun.SetActive(false);
                    _playerWeapon.Flamethrower.SetActive(false);
                    _playerWeapon.CurrentType = Weapon.Type.ShootingWeapon;
                    _playerWeapon.SpreadAngle = 90f;
                    _playerWeapon.Damage = 1;
                    break;
                case "Railgun":
                    _playerWeapon.CurrentWeaponAttack = _playerWeapon.ShootLikeRailgun;
                    _playerWeapon.Source.clip = _playerWeapon.RailgunShotSound;
                    _playerWeapon.Flamethrower.SetActive(false);
                    _playerWeapon.RailgunHolder.SetActive(true);
                    _playerWeapon.CurrentType = Weapon.Type.ChargingWeapon;
                    _playerWeapon.Damage = 1;
                    break;
                case "Flamethrower":
                    _playerWeapon.CurrentWeaponAttack = _playerWeapon.ShootLikeFlamethrower;
                    _playerWeapon.Source.clip = _playerWeapon.FlamethrowerSound;
                    _playerWeapon.Source.loop = true;
                    _playerWeapon.Railgun.SetActive(false);
                    _playerWeapon.Flamethrower.SetActive(true);
                    _playerWeapon.CurrentType = Weapon.Type.HoldingWeapon;
                    _playerWeapon.Damage = 1;
                    break;
                case "ShotgunSpreadUpgrade":
                    _playerWeapon.SpreadAngle = 40f;
                    break;
                case "ShotgunNumberUpgrade":
                    _playerWeapon.BulletsPerShotgunShot = 10;
                    break;
                case "ShotgunDamageUpgrade":
                    _playerWeapon.Damage = 2;
                    break;
                case "ShotgunCooldownUpgrade":
                    _playerWeapon.Cooldown = 0.5f;
                    break;
                default: break;
            }
        }
    }
}

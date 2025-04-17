
using System;
using System.Collections.Generic;

class Program
{
    class Enemy
    {
        public string Name;
        public int MaxHP;
        public int CurrentHP;
        public int Attack;
        public int GoldDrop;

        public Enemy(string name, int hp, int atk, int gold)
        {
            Name = name;
            MaxHP = hp;
            CurrentHP = hp;
            Attack = atk;
            GoldDrop = gold;
        }
    }

    static void Main(string[] args)
    {
        string playerName = "용사";
        int playerHP = 100;
        int maxHP = 100;
        int playerAttack = 20;
        int playerGold = 100;
        int weaponUpgradeLevel = 0;
        int[] upgradeCosts = { 20, 40, 80, 160, 320 };
        Dictionary<string, int> inventory = new Dictionary<string, int>
        {
            {"하급", 0},
            {"중급", 0},
            {"고급", 0}
        };

        Dictionary<string, Enemy> dungeons = new Dictionary<string, Enemy>()
        {
            {"1", new Enemy("고블린", 60, 15, 15)},
            {"2", new Enemy("오크", 90, 20, 30)},
            {"3", new Enemy("하이오크", 200, 30, 100)}
        };

        Console.WriteLine("=== 텍스트 턴제 RPG ===");

        while (true)
        {
            Console.WriteLine("\n=== 던전 선택 ===");
            Console.WriteLine("1. 고블린 던전");
            Console.WriteLine("2. 오크 던전");
            Console.WriteLine("3. 하이오크 던전");
            Console.WriteLine("0. 게임 종료");
            Console.Write("던전을 선택하세요: ");
            string dungeonChoice = Console.ReadLine();

            if (dungeonChoice == "0")
                break;

            if (!dungeons.ContainsKey(dungeonChoice))
            {
                Console.WriteLine("잘못된 선택입니다.");
                continue;
            }

            Enemy enemy = dungeons[dungeonChoice];
            enemy.CurrentHP = enemy.MaxHP;
            Console.WriteLine($"\n>> {enemy.Name} 던전에 입장합니다!");

            while (playerHP > 0 && enemy.CurrentHP > 0)
            {
                Console.WriteLine($"\n{playerName} HP: {playerHP}/{maxHP} | {enemy.Name} HP: {enemy.CurrentHP}/{enemy.MaxHP}");
                Console.WriteLine("1. 공격");
                Console.WriteLine("2. 포션 사용");
                Console.Write("선택: ");
                string action = Console.ReadLine();

                if (action == "1")
                {
                    Console.WriteLine($"{playerName}이(가) {enemy.Name}에게 {playerAttack}의 데미지를 입혔습니다!");
                    enemy.CurrentHP -= playerAttack;
                }
                else if (action == "2")
                {
                    UsePotion(ref playerHP, maxHP, inventory);
                    continue; // 포션 사용 시 몬스터는 공격 안 함
                }
                else
                {
                    Console.WriteLine("잘못된 선택입니다.");
                    continue;
                }

                if (enemy.CurrentHP <= 0)
                {
                    Console.WriteLine($"{enemy.Name}을(를) 쓰러뜨렸습니다!");
                    playerGold += enemy.GoldDrop;
                    Console.WriteLine($"{enemy.GoldDrop} 골드를 획득했습니다! (보유 골드: {playerGold})");
                    break;
                }

                Console.WriteLine($"{enemy.Name}의 공격! {enemy.Attack} 데미지!");
                playerHP -= enemy.Attack;

                if (playerHP <= 0)
                {
                    Console.WriteLine($"{playerName}이(가) 쓰러졌습니다...");
                    Console.WriteLine("게임 오버");
                    return;
                }
            }

            while (true)
            {
                Console.WriteLine("\n어디로 가시겠습니까?");
                Console.WriteLine("1. 잡화상점");
                Console.WriteLine("2. 대장간");
                Console.WriteLine("3. 인벤토리 보기");
                Console.WriteLine("0. 다음으로");
                Console.Write("선택: ");
                string postChoice = Console.ReadLine();

                if (postChoice == "1")
                    OpenShop(ref playerGold, inventory);
                else if (postChoice == "2")
                    OpenBlacksmith(ref playerGold, ref playerAttack, ref weaponUpgradeLevel, upgradeCosts);
                else if (postChoice == "3")
                    ShowInventory(inventory);
                else if (postChoice == "0")
                    break;
                else
                    Console.WriteLine("잘못된 입력입니다.");
            }
        }

        Console.WriteLine("게임을 종료합니다.");
    }

    static void OpenShop(ref int gold, Dictionary<string, int> inventory)
    {
        while (true)
        {
            Console.WriteLine("\n=== 잡화상점 ===");
            Console.WriteLine("1. 하급 회복 포션 (10G) : +20 HP");
            Console.WriteLine("2. 중급 회복 포션 (20G) : +50 HP");
            Console.WriteLine("3. 고급 회복 포션 (30G) : +100 HP");
            Console.WriteLine("0. 상점 나가기");
            Console.WriteLine($"보유 골드: {gold}");
            Console.Write("선택: ");
            string input = Console.ReadLine();

            int cost = 0;
            string potionType = "";

            switch (input)
            {
                case "1": cost = 10; potionType = "하급"; break;
                case "2": cost = 20; potionType = "중급"; break;
                case "3": cost = 30; potionType = "고급"; break;
                case "0": return;
                default: Console.WriteLine("잘못된 입력입니다."); continue;
            }

            if (gold >= cost)
            {
                gold -= cost;
                inventory[potionType]++;
                Console.WriteLine($"{potionType} 포션을 구매했습니다. (남은 골드: {gold})");
            }
            else
            {
                Console.WriteLine("골드가 부족합니다.");
            }
        }
    }

    static void UsePotion(ref int hp, int maxHP, Dictionary<string, int> inventory)
    {
        Console.WriteLine("\n=== 포션 사용 ===");
        ShowInventory(inventory);
        Console.WriteLine("사용할 포션을 선택하세요:");
        Console.WriteLine("1. 하급 (+20 HP)");
        Console.WriteLine("2. 중급 (+50 HP)");
        Console.WriteLine("3. 고급 (+100 HP)");
        Console.WriteLine("0. 취소");
        Console.Write("선택: ");
        string input = Console.ReadLine();

        int heal = 0;
        string type = "";

        switch (input)
        {
            case "1": type = "하급"; heal = 20; break;
            case "2": type = "중급"; heal = 50; break;
            case "3": type = "고급"; heal = 100; break;
            case "0": return;
            default: Console.WriteLine("잘못된 입력입니다."); return;
        }

        if (inventory[type] > 0)
        {
            inventory[type]--;
            hp += heal;
            if (hp > maxHP) hp = maxHP;
            Console.WriteLine($"{type} 포션을 사용했습니다! 현재 HP: {hp}");
        }
        else
        {
            Console.WriteLine($"{type} 포션이 없습니다!");
        }
    }

    static void ShowInventory(Dictionary<string, int> inventory)
    {
        Console.WriteLine("\n=== 인벤토리 ===");
        foreach (var item in inventory)
        {
            Console.WriteLine($"{item.Key} 포션: {item.Value}개");
        }
    }

    static void OpenBlacksmith(ref int gold, ref int attack, ref int level, int[] costs)
    {
        Console.WriteLine("\n=== 대장간 ===");
        if (level >= costs.Length)
        {
            Console.WriteLine("무기는 이미 최고 강화 상태입니다!");
            return;
        }

        int cost = costs[level];
        Console.WriteLine($"현재 공격력: {attack} / 강화 비용: {cost}G");
        Console.Write("강화하시겠습니까? (y/n): ");
        string input = Console.ReadLine();

        if (input.ToLower() == "y")
        {
            if (gold >= cost)
            {
                gold -= cost;
                attack += 10;
                level++;
                Console.WriteLine($"강화 성공! 현재 공격력: {attack} (남은 골드: {gold})");
            }
            else
            {
                Console.WriteLine("골드가 부족합니다.");
            }
        }
    }
}

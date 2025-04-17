
// 턴제 RPG 통합 예제 - 이벤트/델리게이트로 전투 흐름 관리
using System;
using System.Collections.Generic;

namespace TextRPG
{
    // 전투 행동 델리게이트 정의
    public delegate void BattleAction();

    // 플레이어 클래스
    public class Player
    {
        public int HP { get; set; } = 100;
        public int MaxHP { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Gold { get; set; } = 0;
        public int PotionCount { get; set; } = 0;
        public int SwordLevel { get; set; } = 0;

        public void UsePotion()
        {
            if (PotionCount <= 0)
            {
                Console.WriteLine("포션이 없습니다!");
                return;
            }
            int heal = 30;
            HP += heal;
            if (HP > MaxHP) HP = MaxHP;
            PotionCount--;
            Console.WriteLine($"포션 사용! HP +{heal} (현재 HP: {HP})");
        }

        public void UpgradeSword()
        {
            int cost = (int)Math.Pow(2, SwordLevel) * 20;
            if (SwordLevel >= 5)
            {
                Console.WriteLine("이미 최대 강화입니다!");
            }
            else if (Gold >= cost)
            {
                Gold -= cost;
                SwordLevel++;
                Attack += 10;
                Console.WriteLine($"용사의 검을 강화했습니다! 공격력 +10 (현재 공격력: {Attack}, 골드: {Gold})");
            }
            else
            {
                Console.WriteLine("골드가 부족합니다!");
            }
        }
    }

    // 몬스터 클래스
    public class Monster
    {
        public string Name;
        public int HP;
        public int Attack;
        public int GoldReward;

        public Monster(string name, int hp, int atk, int gold)
        {
            Name = name;
            HP = hp;
            Attack = atk;
            GoldReward = gold;
        }
    }

    // 던전 클래스
    public class Dungeon
    {
        public string Name;
        public Monster Monster;

        public Dungeon(string name, Monster monster)
        {
            Name = name;
            Monster = monster;
        }
    }

    // 전투 시스템
    public class BattleSystem
    {
        public event BattleAction OnPlayerAttack;
        public event BattleAction OnUsePotion;
        public event BattleAction OnMonsterAttack;

        private Player player;
        private Monster monster;

        public BattleSystem(Player player, Monster monster)
        {
            this.player = player;
            this.monster = monster;

            // 이벤트 구독
            OnPlayerAttack += () =>
            {
                Console.WriteLine("\n[플레이어의 공격]");
                monster.HP -= player.Attack;
                Console.WriteLine($"{monster.Name}에게 {player.Attack} 데미지를 입혔습니다. 남은 HP: {monster.HP}");
            };

            OnUsePotion += () => player.UsePotion();

            OnMonsterAttack += () =>
            {
                Console.WriteLine($"\n[{monster.Name}의 반격!]");
                player.HP -= monster.Attack;
                Console.WriteLine($"플레이어가 {monster.Attack} 데미지를 입었습니다. 현재 HP: {player.HP}");
            };
        }

        public void StartBattle()
        {
            Console.WriteLine($"{monster.Name}와의 전투가 시작됩니다!");
            while (player.HP > 0 && monster.HP > 0)
            {
                Console.WriteLine("\n1. 공격\n2. 포션 사용");
                Console.Write("행동 선택: ");
                string input = Console.ReadLine();

                if (input == "1") OnPlayerAttack?.Invoke();
                else if (input == "2") OnUsePotion?.Invoke();
                else
                {
                    Console.WriteLine("잘못된 입력");
                    continue;
                }

                if (monster.HP <= 0)
                {
                    Console.WriteLine($"{monster.Name}을(를) 쓰러뜨렸습니다! 골드 +{monster.GoldReward}");
                    player.Gold += monster.GoldReward;
                    break;
                }

                OnMonsterAttack?.Invoke();
                if (player.HP <= 0)
                {
                    Console.WriteLine("플레이어가 쓰러졌습니다...");
                    break;
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Player player = new Player();

            List<Dungeon> dungeons = new List<Dungeon>
            {
                new Dungeon("고블린 던전", new Monster("고블린", 50, 10, 15)),
                new Dungeon("오크 던전", new Monster("오크", 90, 20, 30)),
                new Dungeon("하이오크 던전", new Monster("하이오크", 200, 30, 100)),
            };

            while (true)
            {
                Console.WriteLine("\n--- 던전을 선택하세요 ---");
                for (int i = 0; i < dungeons.Count; i++)
                    Console.WriteLine($"{i + 1}. {dungeons[i].Name}");
                Console.Write("선택: ");
                string input = Console.ReadLine();
                if (!int.TryParse(input, out int dungeonChoice) || dungeonChoice < 1 || dungeonChoice > dungeons.Count)
                {
                    Console.WriteLine("잘못된 선택");
                    continue;
                }

                var selectedDungeon = dungeons[dungeonChoice - 1];
                BattleSystem battle = new BattleSystem(player, selectedDungeon.Monster);
                battle.StartBattle();

                if (player.HP <= 0) break;

                Console.WriteLine("\n전투 후 행동 선택:");
                Console.WriteLine("1. 잡화상점 가기");
                Console.WriteLine("2. 대장간 가기");
                Console.WriteLine("3. 다음 던전으로");
                Console.Write("선택: ");
                string choice = Console.ReadLine();

                if (choice == "1") // 잡화상점
                {
                    Console.WriteLine("\n잡화상점 - 포션 가격: 하급 10G, 중급 20G, 고급 30G");
                    Console.Write("몇 개 구매하시겠습니까? (하급, 10G): ");
                    if (int.TryParse(Console.ReadLine(), out int amount))
                    {
                        int cost = amount * 10;
                        if (player.Gold >= cost)
                        {
                            player.Gold -= cost;
                            player.PotionCount += amount;
                            Console.WriteLine($"포션 {amount}개 구매 완료. 남은 골드: {player.Gold}");
                        }
                        else Console.WriteLine("골드 부족");
                    }
                }
                else if (choice == "2") // 대장간
                {
                    player.UpgradeSword();
                }
            }

            Console.WriteLine("게임 종료");
        }
    }
}

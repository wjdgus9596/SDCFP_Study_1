using System;

using System;
using System.Collections.Generic;

using System;
using System.Collections.Generic;

class Program
{
    static Dictionary<int, (string name, int price)> menu = new Dictionary<int, (string, int)>
    {
        { 1, ("초밥세트", 12000) },
        { 2, ("우동", 8000) },
        { 3, ("돈까스", 9000) },
        { 4, ("라멘", 9500) },
        { 5, ("사케동", 11000) }
    };

    static Dictionary<string, string[]> menuOptions = new Dictionary<string, string[]>
    {
        { "우동", new[] { "오뎅", "유부" } },
        { "돈까스", new[] { "카레", "냉모밀" } },
        { "라멘", new[] { "차슈", "계란" } },
        { "사케동", new[] { "마늘 푸레이크", "고기 추가" } }
    };

    static void Main(string[] args)
    {
        List<(string name, int price)> orderList = new List<(string, int)>();
        int totalPrice = 0;

        Console.WriteLine("=== 일식 음식 키오스크 ===");

        while (true)
        {
            DisplayMenu();
            Console.Write("메뉴 번호를 선택하세요 (주문 완료: 0): ");
            string input = Console.ReadLine();

            if (int.TryParse(input, out int menuNumber))
            {
                if (menuNumber == 0)
                    break;

                if (menu.ContainsKey(menuNumber))
                {
                    var selected = menu[menuNumber];
                    string optionText = SelectMenuOptions(selected.name);
                    string fullName = selected.name + optionText;
                    orderList.Add((fullName, selected.price));
                    totalPrice += selected.price;
                    Console.WriteLine($"{fullName} 선택됨 - 가격: {selected.price}원");
                }
                else
                {
                    Console.WriteLine("존재하지 않는 메뉴입니다.");
                }
            }
            else
            {
                Console.WriteLine("숫자를 입력해주세요.");
            }

            Console.WriteLine();
        }

        if (orderList.Count == 0)
        {
            Console.WriteLine("주문이 없습니다. 프로그램을 종료합니다.");
            return;
        }

        // 이벤트
        int discount = 0;
        double percentageDiscount = 0.0;
        bool freeDrink = false;

        Console.WriteLine("\n=== 리뷰 이벤트 참여하시겠습니까? ===");
        Console.WriteLine("1. 블로그 리뷰 (사진 포함) - 10% 할인");
        Console.WriteLine("2. 네이버 리뷰 - 3,000원 할인");
        Console.WriteLine("3. 인스타그램 팔로우 - 음료수 서비스 제공");
        Console.WriteLine("0. 참여하지 않음");
        Console.Write("선택: ");
        string eventInput = Console.ReadLine();

        switch (eventInput)
        {
            case "1":
                percentageDiscount = 0.10;
                Console.WriteLine("블로그 리뷰 이벤트 선택 - 10% 할인 적용됩니다.");
                break;
            case "2":
                discount = 3000;
                Console.WriteLine("네이버 리뷰 이벤트 선택 - 3,000원 할인 적용됩니다.");
                break;
            case "3":
                freeDrink = true;
                Console.WriteLine("인스타그램 팔로우 이벤트 선택 - 음료수 서비스 제공됩니다!");
                break;
            default:
                Console.WriteLine("이벤트에 참여하지 않았습니다.");
                break;
        }

        // 할인 적용
        int discountedPrice = totalPrice;

        if (percentageDiscount > 0)
        {
            discountedPrice = (int)(totalPrice * (1 - percentageDiscount));
        }
        else
        {
            discountedPrice = totalPrice - discount;
            if (discountedPrice < 0) discountedPrice = 0;
        }

        DisplayOrderSummary(orderList, totalPrice, discountedPrice, freeDrink);

        // 결제
        Console.WriteLine("\n=== 결제 수단 선택 ===");
        Console.WriteLine("1. 카드");
        Console.WriteLine("2. 현금");
        Console.WriteLine("3. 간편 결제 (네이버페이/카카오페이)");
        Console.WriteLine("4. 스마트 페이 (삼성페이/애플페이)");
        Console.Write("선택: ");
        string paymentMethod = Console.ReadLine();

        string paymentType = paymentMethod switch
        {
            "1" => "카드",
            "2" => "현금",
            "3" => "간편 결제",
            "4" => "스마트 페이",
            _ => "기본 결제"
        };

        Console.WriteLine($"[{paymentType}] 결제가 완료되었습니다. 총 결제금액: {discountedPrice}원");

        // 직원 호출
        Console.WriteLine("\n=== 직원 호출 기능 ===");
        Console.Write("직원을 호출하시겠습니까? (y/n): ");
        string callStaff = Console.ReadLine()?.Trim().ToLower();

        if (callStaff == "y")
        {
            CallStaff();
        }

        Console.WriteLine("\n감사합니다. 맛있게 드세요!");
    }

    static string SelectMenuOptions(string menuName)
    {
        if (!menuOptions.ContainsKey(menuName))
            return "";

        var options = menuOptions[menuName];
        Console.WriteLine($"{menuName} 옵션 선택:");
        for (int i = 0; i < options.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i]}");
        }
        Console.Write("옵션을 선택하세요 (쉼표로 복수 선택 가능, 예: 1,2): ");
        string input = Console.ReadLine();
        List<string> selected = new List<string>();

        if (!string.IsNullOrWhiteSpace(input))
        {
            var parts = input.Split(',');
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int index) &&
                    index >= 1 && index <= options.Length)
                {
                    selected.Add(options[index - 1]);
                }
            }
        }

        if (selected.Count > 0)
        {
            return $" ({string.Join(", ", selected)})";
        }

        return "";
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\n--- 메뉴판 ---");
        foreach (var item in menu)
        {
            Console.WriteLine($"{item.Key}. {item.Value.name} - {item.Value.price}원");
        }
    }

    static void DisplayOrderSummary(List<(string name, int price)> orders, int originalTotal, int discountedTotal, bool includeDrink)
    {
        Console.WriteLine("\n=== 주문 내역 ===");
        foreach (var order in orders)
        {
            Console.WriteLine($"{order.name} - {order.price}원");
        }

        Console.WriteLine($"원래 총액: {originalTotal}원");
        Console.WriteLine($"할인 적용 총액: {discountedTotal}원");

        if (includeDrink)
        {
            Console.WriteLine("무료 음료가 함께 제공됩니다! (매장에서 확인하세요)");
        }
    }

    static void CallStaff()
    {
        Console.WriteLine("\n--- 요청 가능한 항목 ---");
        string[] items = { "물수건", "소주", "맥주", "숟가락", "젓가락", "물", "밑반찬" };
        for (int i = 0; i < items.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i]}");
        }

        Console.Write("요청할 항목 번호 입력 (쉼표로 구분): ");
        string input = Console.ReadLine();
        var selectedItems = new List<string>();

        if (!string.IsNullOrWhiteSpace(input))
        {
            var parts = input.Split(',');
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int index) && index >= 1 && index <= items.Length)
                {
                    selectedItems.Add(items[index - 1]);
                }
            }
        }

        if (selectedItems.Count > 0)
        {
            Console.WriteLine("아래 요청이 접수되었습니다:");
            foreach (var item in selectedItems)
            {
                Console.WriteLine($"- {item}");
            }
        }
        else
        {
            Console.WriteLine("요청 항목이 없습니다.");
        }
    }
}
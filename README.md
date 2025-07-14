# C# WPF project

idea

### 카페관리 프로그램 생각 정리

C# + WPF MVVM 패턴을 사용할 예정!

크게 손님 View / 주방 View / 재고관리 View 매출관리 View 로 나뉜다.

손님 View : 

- 5개의 메뉴가 존재하고 손님이 클릭하면 장바구니에 담기고 주문버튼을 누르면

장바구니가 비워지면서 주방으로 Data가 전달됨, 성공적으로 주문이 되었다면

“주문이 접수되었습니다.” 라는 멘트가 뜸

//카페관리 프로그램

1. 주문

식당에 앉은 손님이 메뉴를 선택하여 주문을 한다. (table number, memu, price …)

손님이 보는 화면은 메뉴의 종류와 가격이 나와있는 WPF 화면이다.

주방에서는 들어온 주문의 목록을 순서대로 볼수 있다, 동일한 메뉴가 들어온 경우 같은 색으로 출력한다

주방에서 요리가 완료되면 완료 버튼을 누른다.

아르바이트생은 어떤 요리가 완성되어있는지를 볼 수 있고 해당 음식을 손님에게 전달한다.

1. 창고관리

매니저는 매일 저녁 남아있는 재료의 수량을 체크하고 주문을 넣고자 한다.

주방에서 요리 완성 버튼을 누르는 순간 해당하는 재료만큼 재고에서 뺀다.

창고에 들어있는 요리의 수량이 부족하다면 손님의 화면에 해당 메뉴는 재료품절이라고 나온다.

주문한 상품이 도착하면 매니저는 수량을 확인하여 수동으로 기입한다.

일일 매출

금일 예상 소비량

재고목록

재고 입고 버튼

재고 수정 버튼

재고 출고 버튼

재고 속성

id 이름 가격 수량 입고일

입고

재고 속성

이름 가격 수량 소비량

재고목록 표시될거고

1. 정산

매니저는 화면에서 한달간 총 수입, 영업이익, 당일 수익을 볼 수 있다.

고려해야 할 항목

음식을 판매함으로서 벌어들인 수입, 재료비

고정지출 비용 (월세, 전기세, 수도세, 아르바이트생 월급) 

오전에 주문받았던 항목들로 부터 각각의 금액을 정산한다. 

컨트롤 : 

객체 

이벤트

메뉴:

에소프레소(원두 2) 2500원

아메리카노(원두 2) 2500원

헤이즐넛아메리카노(원두 2, 헤이즐넛 시럽 1) 3000원

카페라떼(원두 2, 우유 2) 3500원

바닐라라떼(원두 2, 우유 2, 바닐라시럽 1) 4000원

# 🧱 클래스 설계 예시

### 1. `Ingredient` – 재고 단위

```csharp

public class Ingredient
{
    public string Name { get; set; }      // ex: 원두, 우유, 시럽
    public int Stock { get; set; }        // 현재 재고 수량
}

```

---

### 2. `RecipeItem` – 메뉴 1개에 포함되는 재료 1개

```csharp

public class RecipeItem
{
    public Ingredient Ingredient { get; set; }  // 어떤 재료
    public int Quantity { get; set; }           // 몇 개 들어가는지
}

```

---

### 3. `MenuItem` – 한 메뉴

```csharp

public class MenuItem
{
    public string Name { get; set; }                 // ex: 바닐라라떼
    public int Price { get; set; }                   // ex: 4000
    public List<RecipeItem> Recipe { get; set; }     // 이 메뉴에 들어가는 재료 목록
}

```

```csharp

```

### 재고 관리

```csharp
//재고관리 

class Model(product) , 
{
	private static int cnt; // id 부여를 위해 사용
	private int Id
	private string Name;
	private int Price;
	private int Quantity;
	private int Date_of_receipt;
	private int Consumption;
	
	public Product(string name, int price, int quantity, int date_of_receipt)
  {
       Id = ++cnt;
       Name = name;
       Price = price;
       Quantity = quantity;
       Date_of_receipt = date_of_receipt;
       Consumption = 0;
   }
   
	stock_in
	
	modify
	
	del_prod
		
}

/* 준비해야할 것
product 속성 (이름/가격/수량/입고일) 입력 후 입고 버튼 클릭 시 입고
product list에서 하나 클릭하면 아래의 속성 입력 창에 해당 product의 수정 가능 정보가 채워짐
product list에서 하나 클릭한 상태에서 아래의 속성 입력 창에 정보 수정 후 수정 누를 시 수정
product list에서 하나 클릭한 상태에서 삭제 버튼 누를 시 해당 product 삭제

재고관리에서 매출관리로 탭 버튼 누를 시 넘어가는 이벤트까지
*/

class viewModel
{
	
}

class Command
{
	
}
```

```csharp
//view
```

---

### 프로젝트 개요


WPF_Cafeteria_Project
WPF_Cafeteria_Project는 **WPF(Windows Presentation Foundation)**를 활용하여 개발된 식당/카페테리아 관리 시스템입니다. 이 프로젝트는 주문 처리, 메뉴 관리, 테이블 관리 등 식당 운영에 필요한 핵심 기능을 제공하며, 사용자 친화적인 GUI를 통해 효율적인 작업 환경을 지원합니다. 4명의 팀원이 각자 담당 모듈을 개발하여 완성한 팀 프로젝트입니다.

🌟 주요 기능
주문 관리: 고객 주문을 손쉽게 등록, 수정, 삭제하고, 주문 내역을 조회할 수 있습니다.

메뉴 관리: 메뉴 항목을 추가, 편집, 삭제하고, 카테고리별로 분류하여 효율적으로 관리할 수 있습니다.

테이블 관리: 식당 내 테이블의 실시간 상태(예: 빈 테이블, 사용 중, 결제 대기)를 직관적으로 파악하고, 테이블 간 이동 및 합석 기능을 지원합니다.

결제 시스템: 현금, 카드 등 다양한 결제 방식을 지원하며, 영수증 출력 기능을 포함합니다.

사용자 인증 및 권한 관리: 관리자 및 직원 계정으로 로그인하여 시스템에 접근할 수 있으며, 각 역할에 따른 접근 권한을 관리합니다.

🚀 기술 스택
프론트엔드: WPF (XAML, C#)

백엔드: (프로젝트에 사용된 백엔드 기술을 여기에 기재하세요. 예: C# .NET Core Web API, Python Flask 등)

데이터베이스: (프로젝트에 사용된 데이터베이스를 여기에 기재하세요. 예: SQLite, SQL Server, MySQL 등)

🧑‍💻 팀원 및 담당 역할
이 프로젝트는 4명의 팀원이 각자의 역할을 분담하여 개발되었습니다.

[팀원 1 이름]: (담당 모듈/기능 명시, 예: 주문 관리 및 결제 시스템 개발)

[팀원 2 이름]: (담당 모듈/기능 명시, 예: 메뉴 관리 및 데이터베이스 연동)

[팀원 3 이름]: (담당 모듈/기능 명시, 예: 테이블 관리 및 GUI/UX 디자인)

[팀원 4 이름]: (담당 모듈/기능 명시, 예: 사용자 인증 및 권한 관리, 보고서 기능)

카페 관리 프로그램. 뷰는 손님, 주방, 관리용 총 3개. 각각의 뷰는 TCP 통신을 통해 데이터 주고 받음.

(손님 -(주문)→ 주방 -(재고 소진/매출 발생)→ 관리)

메뉴는 총 4개이며 각 메뉴마다 가격, 들어가는 재료가 다름.

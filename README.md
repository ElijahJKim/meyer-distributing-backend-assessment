nus 밖에 한 것 (“creative / 추가”)
회사 쪽 **“SQLite + 창의적으로 확장”**에 맞춘 것:

1. CustomerActivityReport (가장 큰 추가)
고객별 주문/반품 히스토리 콘솔 출력
구매일, 반품 여부, 반품일
Program.cs 주석에는 없음
2. ReturnedAt
반품 시각 기록 (Return.AddProduct → ReturnedAt)
Activity Report용. 요구 3번은 구매 시각만 명시
3. OrderedProduct.Id + last_insert_rowid()
주문 한 줄 단위 DB id
같은 상품 2줄 + 그중 하나만 반품 시나리오 대비
과제 예제만 보면 필수는 아님
4. 앱 시작 시 Orders 등 DELETE (DatabaseInitializer)
재실행할 때 예제 주문 중복 방지
5. 제출/개발용
.gitignore (bin/, obj/, *.db)
테스트용 DatabasePaths.SetDatabasePath()
DbProduct (DB에서 읽은 일반 상품 타입)필수·주석 Bo
6. ProcessExchange (교환)
기존 주문 줄 반품 + 새 주문 생성, 차액 정산 (예: 침대 라이너 $150 반품 → 히치 $70 + 오일 $25 재주문 → $55 환불)

## Future improvements

### `ProductNumber` 대신 내부 `Id`로 상품 조회

현재 `Program.cs`는 부품 번호로 상품을 가져온다 (예: `productRepo.GetByProductNumber("DrawTite 5504")`). `Products` 테이블은 `ProductNumber`를 primary key로 쓴다.

실서비스에서는 보통 이렇게 한다:

- `Products`에 integer `Id`를 PK로 추가하고, `ProductNumber`는 unique business key(SKU)로 둔다.
- API는 클라이언트에서 `productId`를 받고, 서버가 DB에서 상품을 조회한 뒤 주문에 넣는다.
- `OrderProducts`는 `ProductId`(FK)를 참조하고, 주문 시점 `SellingPrice`는 스냅샷으로 저장한다.

B2B 유통에서는 부품 번호(SKU)로 주문하는 경우도 많다. 외부 연동은 SKU를 쓰되, 내부 FK와 API는 integer Id를 쓰는 패턴이 일반적이다.

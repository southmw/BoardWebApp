# BoardApp.WebApp

Blazor WebAssembly와 Interactive Server를 결합한 하이브리드 렌더링 모드를 사용하는 현대적인 게시판 웹 애플리케이션입니다.

![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MudBlazor](https://img.shields.io/badge/MudBlazor-8.13.0-594AE2?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![ASP.NET Identity](https://img.shields.io/badge/ASP.NET%20Identity-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## 주요 기능

### 인증 및 권한 관리
- **ASP.NET Identity 통합**: 완전한 사용자 인증 및 권한 관리 시스템
- **역할 기반 접근 제어**: Admin, User 역할을 통한 권한 관리
- **Google OAuth 로그인**: 소셜 로그인 지원 (설정 필요)
- **프로필 관리**: 사용자 프로필, 아바타, 자기소개 편집

### 게시판 기능
- **게시글 CRUD**: 게시글 생성, 조회, 수정, 삭제
- **카테고리 분류**: 게시글을 카테고리별로 조직화
- **리치 텍스트 에디터**: Quill 에디터를 통한 다양한 서식 지원
- **파일 첨부**: 이미지, 비디오, 문서 등 다양한 파일 업로드 및 미리보기
- **조회수 추적**: 게시글별 조회수 자동 집계
- **페이징**: 효율적인 게시글 목록 페이징

### 댓글 기능
- **댓글 작성**: 게시글에 댓글 작성
- **댓글 수정/삭제**: 작성한 댓글 수정 및 삭제
- **실시간 댓글 수 표시**: 게시글 목록에서 댓글 수 확인
- **댓글 작성자 아바타**: 작성자별 고유 아바타 표시

### 카테고리 관리
- **카테고리 생성/수정/삭제**: 관리자 전용 카테고리 관리
- **카테고리 색상**: 각 카테고리별 고유 색상 지정
- **카테고리 고정**: 중요한 카테고리 상단 고정
- **활성화/비활성화**: 카테고리 표시 여부 제어
- **정렬 순서**: DisplayOrder를 통한 카테고리 정렬

### UI/UX 기능
- **현대적인 디자인**: 그라데이션과 둥근 모서리를 활용한 모던 디자인
- **반응형 레이아웃**: 모바일, 태블릿, 데스크톱 모든 화면 크기 지원
- **통계 대시보드**: 전체 게시글, 오늘 작성글, 전체 조회수 통계
- **인터랙티브 효과**: Hover, 애니메이션 등 다양한 인터랙션
- **다크/라이트 모드 준비**: 테마 시스템 적용

### 기술 기능
- **하이브리드 렌더링**: WebAssembly와 Server 모드 혼합
- **자동 마이그레이션**: 애플리케이션 시작 시 DB 자동 마이그레이션
- **자동 시드 데이터**: 역할 및 관리자 계정 자동 생성
- **파일 관리**: 체계적인 파일 업로드 및 저장 시스템
- **에러 핸들링**: 사용자 친화적인 에러 메시지

## 프로젝트 구조

```
BoardApp/
├── BoardApp.WebApp/              # 서버 측 프로젝트
│   ├── Components/              # Blazor 컴포넌트
│   │   ├── Pages/              # 페이지 컴포넌트
│   │   │   ├── Account/        # 인증 관련 페이지
│   │   │   │   ├── Login.razor
│   │   │   │   ├── Register.razor
│   │   │   │   └── Profile.razor
│   │   │   ├── Admin/          # 관리자 페이지
│   │   │   │   └── Categories.razor
│   │   │   ├── Boards/         # 게시판 관련 페이지
│   │   │   │   ├── Index.razor    # 게시판 목록
│   │   │   │   ├── Detail.razor   # 게시글 상세
│   │   │   │   ├── Create.razor   # 게시글 작성
│   │   │   │   └── Edit.razor     # 게시글 수정
│   │   │   ├── Home.razor      # 홈 페이지
│   │   │   └── Weather.razor   # 날씨 페이지
│   │   └── Layout/             # 레이아웃 컴포넌트
│   │       ├── MainLayout.razor
│   │       └── NavMenu.razor
│   ├── Data/                   # 데이터베이스 컨텍스트
│   │   └── ApplicationDbContext.cs
│   ├── Models/                 # 데이터 모델
│   │   ├── ApplicationUser.cs  # Identity 사용자 모델
│   │   ├── ApplicationRole.cs  # Identity 역할 모델
│   │   ├── Board.cs           # 게시글 모델
│   │   ├── BoardAttachment.cs  # 첨부파일 모델
│   │   ├── Category.cs         # 카테고리 모델
│   │   └── Comment.cs          # 댓글 모델
│   ├── Services/               # 비즈니스 로직 서비스
│   │   ├── IBoardService.cs / BoardService.cs
│   │   ├── ICommentService.cs / CommentService.cs
│   │   ├── IFileUploadService.cs / FileUploadService.cs
│   │   ├── ICategoryService.cs / CategoryService.cs
│   │   ├── IUserService.cs / UserService.cs
│   │   ├── IRoleService.cs / RoleService.cs
│   │   └── UserStateService.cs
│   └── Migrations/             # EF Core 마이그레이션
│
└── BoardApp.WebApp.Client/       # 클라이언트 측 프로젝트 (WASM)
    └── Pages/                   # 클라이언트 페이지
        └── Counter.razor        # 카운터 페이지
```

## 기술 스택

- **.NET 9.0**: 최신 .NET 프레임워크
- **Blazor WebAssembly + Interactive Server**: 하이브리드 렌더링 모드
- **ASP.NET Identity**: 사용자 인증 및 권한 관리
- **Entity Framework Core 9.0**: ORM 및 데이터베이스 마이그레이션
- **SQL Server**: 데이터베이스
- **MudBlazor 8.13.0**: Material Design UI 컴포넌트 라이브러리
- **Blazored.TextEditor**: 리치 텍스트 에디터 (Quill.js)
- **Google OAuth**: 소셜 로그인

## 설치 및 실행

### 사전 요구사항

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, Express, 또는 전체 버전)
- Visual Studio 2022 또는 Visual Studio Code (선택 사항)

### 실행 방법

1. **저장소 클론**
```bash
git clone <repository-url>
cd BoardApp
```

2. **데이터베이스 연결 문자열 설정**

`BoardApp.WebApp/appsettings.json` 파일에서 연결 문자열을 환경에 맞게 수정하세요:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SouthmwWebApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

3. **프로젝트 빌드**
```bash
dotnet build
```

4. **애플리케이션 실행**
```bash
cd BoardApp.WebApp
dotnet run
```

또는 Visual Studio에서 `BoardApp.WebApp.sln` 솔루션 파일을 열고 F5를 눌러 실행합니다.

5. **브라우저에서 접속**
```
https://localhost:7203
또는
http://localhost:5173
```

6. **기본 관리자 계정으로 로그인**
```
이메일: admin@southmw.com
비밀번호: Admin123!
```

### Google OAuth 설정 (선택 사항)

Google 로그인을 사용하려면:

1. [Google Cloud Console](https://console.cloud.google.com/)에서 OAuth 2.0 클라이언트 ID 생성
2. `appsettings.json`에 클라이언트 ID와 시크릿 추가:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

## 데이터베이스 마이그레이션

애플리케이션은 시작 시 자동으로 데이터베이스 마이그레이션을 수행합니다. 수동으로 마이그레이션을 관리하려면:

```bash
# 마이그레이션 추가
cd BoardApp.WebApp
dotnet ef migrations add <MigrationName>

# 데이터베이스 업데이트
dotnet ef database update

# 데이터베이스 삭제 및 재생성
dotnet ef database drop --force
dotnet ef database update
```

## 데이터베이스 스키마

### Identity 테이블 (ASP.NET Identity 표준)
- **AspNetUsers**: 사용자 정보
- **AspNetRoles**: 역할 정보
- **AspNetUserRoles**: 사용자-역할 매핑
- 기타 Identity 관련 테이블

### 애플리케이션 테이블

#### Boards (게시글)
- Id (int, PK)
- Title (nvarchar(200))
- Content (nvarchar(max))
- Author (nvarchar(50))
- AuthorId (nvarchar(450), FK → AspNetUsers)
- CategoryId (int, FK → Categories)
- ViewCount (int)
- CreatedAt (datetime)
- UpdatedAt (datetime, nullable)

#### Comments (댓글)
- Id (int, PK)
- Content (nvarchar(1000))
- Author (nvarchar(50))
- AuthorId (nvarchar(450), FK → AspNetUsers)
- BoardId (int, FK → Boards)
- CreatedAt (datetime)
- UpdatedAt (datetime, nullable)

#### BoardAttachments (첨부파일)
- Id (int, PK)
- FileName (nvarchar(255))
- FilePath (nvarchar(500))
- ContentType (nvarchar(100))
- FileSize (bigint)
- BoardId (int, FK → Boards)
- UploadedAt (datetime)

#### Categories (카테고리)
- Id (int, PK)
- Name (nvarchar(50))
- Description (nvarchar(200))
- Color (nvarchar(7))
- DisplayOrder (int)
- IsPinned (bit)
- IsActive (bit)
- CreatedById (nvarchar(450), FK → AspNetUsers)
- CreatedAt (datetime)
- UpdatedAt (datetime, nullable)

## 개발 환경 설정

### Visual Studio Code

1. C# Dev Kit 확장 설치
2. 프로젝트 폴더 열기
3. F5를 눌러 디버깅 시작

### Visual Studio 2022

1. 솔루션 파일 열기 (`BoardApp.WebApp.sln`)
2. 시작 프로젝트를 `BoardApp.WebApp`으로 설정
3. F5를 눌러 디버깅 시작

## 디자인 시스템

### 색상 팔레트
```css
Primary Gradient: linear-gradient(135deg, #667eea 0%, #764ba2 100%)
Success Gradient: linear-gradient(135deg, #06d6a0 0%, #1b9aaa 100%)
Warning Gradient: linear-gradient(135deg, #ffd166 0%, #ef476f 100%)
Info Gradient: linear-gradient(135deg, #4cc9f0 0%, #4361ee 100%)
Background: linear-gradient(to bottom, #f5f7fa 0%, #c3cfe2 100%)
```

### 주요 색상
- Primary: `#667eea` (보라색)
- Secondary: `#764ba2` (진한 보라색)
- Success: `#06d6a0` (민트)
- Info: `#4cc9f0` (하늘색)
- Warning: `#ffd166` (노란색)
- Error: `#ef476f` (핑크)

### 타이포그래피
- Font Family: Inter, Roboto, Helvetica, Arial, sans-serif
- 헤딩: 600-700 weight
- 본문: 400 weight
- Line Height: 1.5-1.6

### 간격 시스템
- Small: 4px, 8px
- Medium: 12px, 16px
- Large: 20px, 24px, 32px

### Border Radius
- Small: 8px
- Medium: 12px
- Large: 16px

## 아키텍처 패턴

### DbContextFactory 패턴
Interactive Blazor 컴포넌트에서의 스레드 안전성을 위해 `IDbContextFactory<ApplicationDbContext>`를 사용합니다:

```csharp
public class BoardService : IBoardService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public async Task<Board?> GetBoardByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Boards
            .Include(b => b.Category)
            .Include(b => b.User)
            .Include(b => b.Attachments)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}
```

### 서비스 레이어
모든 비즈니스 로직은 서비스 레이어에 캡슐화되어 있습니다:

```
컴포넌트 → 서비스 → DbContext → 데이터베이스
```

### 인증 및 권한
- ASP.NET Identity를 사용한 쿠키 기반 인증
- 역할 기반 접근 제어 (RBAC)
- 정책 기반 권한 부여

## 보안 고려사항

### 구현된 보안 기능
✅ ASP.NET Identity 기반 인증
✅ 역할 기반 접근 제어
✅ 비밀번호 정책 (최소 6자, 대문자 필요, 숫자 필요)
✅ HTTPS 리다이렉션
✅ 앤티포저리 토큰

### 주의사항
⚠️ **HTML Sanitization**: 리치 에디터 콘텐츠가 sanitize되지 않습니다. 프로덕션 배포 전 HtmlSanitizer 추가 필요
⚠️ **파일 업로드**: 클라이언트 측 검증만 구현됨. 서버 측 파일 타입 및 크기 검증 추가 권장
⚠️ **민감 정보**: `appsettings.json`의 연결 문자열 및 OAuth 시크릿을 환경 변수나 Azure Key Vault로 관리 권장

## 구성 옵션

### Kestrel 서버 설정
Program.cs에서 대용량 파일 업로드를 위한 설정:
- MaxRequestBodySize: 100MB
- MaximumReceiveMessageSize (SignalR): 100MB

### 파일 업로드 제약
- 최대 파일 크기: 10 MB (클라이언트 측 검증)
- 업로드당 최대 파일 수: 10개
- 허용 형식: `image/*,video/*,.pdf,.doc,.docx,.xls,.xlsx,.zip`
- 저장 위치: `wwwroot/uploads/boards/{boardId}/`

### MudBlazor 테마
커스텀 테마가 적용되어 있으며, `Components/Layout/MainLayout.razor`에서 색상, 폰트 등을 커스터마이징할 수 있습니다.

## 향후 개발 계획

- [x] ~~사용자 인증 및 권한 관리~~ (완료)
- [x] ~~게시글 카테고리 분류~~ (완료)
- [ ] 게시글 검색 기능 고도화 (전체 텍스트 검색)
- [ ] 좋아요/싫어요 기능
- [ ] 답글 기능 (대댓글)
- [ ] 실시간 알림 (SignalR)
- [ ] 이메일 알림
- [ ] 관리자 대시보드 확장
- [ ] REST API 엔드포인트 추가
- [ ] 다국어 지원 (i18n)
- [ ] 게시글 북마크/즐겨찾기
- [ ] 사용자 팔로우 기능
- [ ] 태그 시스템

## 문제 해결

### 데이터베이스 연결 오류
```bash
# 연결 문자열 확인
# BoardApp.WebApp/appsettings.json의 DefaultConnection 확인

# 마이그레이션 재적용
cd BoardApp.WebApp
dotnet ef database drop --force
dotnet ef database update
```

### 빌드 오류
```bash
# 캐시 정리
dotnet clean
dotnet restore
dotnet build
```

### 포트 충돌
`BoardApp.WebApp/Properties/launchSettings.json`에서 포트 번호를 변경하세요.

### 관리자 계정 로그인 실패
애플리케이션을 다시 시작하면 관리자 계정이 자동으로 생성됩니다. 데이터베이스를 확인하거나 로그를 참조하세요.

## 성능 최적화

- **DbContextFactory 사용**: 스레드 안전한 DB 컨텍스트 관리
- **Static Web Assets**: 정적 파일 최적화
- **Lazy Loading**: 필요한 컴포넌트만 로드
- **비동기 처리**: 모든 DB 작업 비동기 처리
- **페이징**: 대량 데이터 효율적 처리

## 라이선스

이 프로젝트는 학습 및 개인 프로젝트 목적으로 작성되었습니다.

## 기여

버그 리포트 및 기능 제안은 이슈 트래커를 통해 제출해주세요.

## 문의

프로젝트 관련 문의사항이 있으시면 이슈를 생성해주세요.

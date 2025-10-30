# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

**BoardApp.WebApp**은 .NET 9.0 기반의 현대적인 Blazor 게시판 애플리케이션입니다. Blazor WebAssembly와 Interactive Server를 결합한 하이브리드 렌더링 모델을 사용하며, ASP.NET Identity 기반 인증, 게시글, 댓글, 파일 첨부, 카테고리 분류, 리치 텍스트 에디터를 포함한 완전한 게시판 기능을 구현합니다.

## 빌드 및 실행 명령어

### 개발 환경

```bash
# 솔루션 빌드
dotnet build

# 애플리케이션 실행 (루트 또는 BoardApp.WebApp 디렉토리에서)
dotnet run --project BoardApp.WebApp
# 또는
cd BoardApp.WebApp && dotnet run

# 빌드 결과물 정리
dotnet clean
```

애플리케이션 접속 주소:
- HTTPS: https://localhost:7203
- HTTP: http://localhost:5173

기본 관리자 계정:
- 이메일: admin@southmw.com
- 비밀번호: Admin123!

### 데이터베이스 마이그레이션

```bash
# 새 마이그레이션 생성 (BoardApp.WebApp 디렉토리에서)
cd BoardApp.WebApp
dotnet ef migrations add <마이그레이션이름>

# 마이그레이션 수동 적용 (기본적으로 시작 시 자동 적용됨)
dotnet ef database update

# 데이터베이스 삭제 및 재생성
dotnet ef database drop --force
dotnet ef database update

# 마지막 마이그레이션 제거
dotnet ef migrations remove
```

**참고:** 애플리케이션은 시작 시 자동으로 데이터베이스 마이그레이션을 수행합니다([Program.cs:95-110](BoardApp.WebApp/Program.cs#L95-L110) 참조). 따라서 일반적으로 수동 마이그레이션은 필요하지 않습니다.

## 아키텍처 개요

### 프로젝트 구조

이 솔루션은 두 개의 주요 프로젝트로 구성된 **멀티 프로젝트 솔루션**입니다:

1. **BoardApp.WebApp** (서버) - ASP.NET Core Web SDK
   - Blazor Interactive Server 컴포넌트
   - ASP.NET Identity 인증 및 권한 관리
   - Entity Framework Core 데이터 액세스
   - 비즈니스 로직 서비스
   - 파일 업로드 처리
   - 데이터베이스 컨텍스트 및 마이그레이션

2. **BoardApp.WebApp.Client** (WASM) - Blazor WebAssembly SDK
   - 클라이언트 측 컴포넌트
   - 현재는 최소한의 구성 (Counter 예제)
   - 클라이언트 전용 기능을 위해 확장 가능

### 핵심 패턴

#### ASP.NET Identity 통합

이 애플리케이션은 **ASP.NET Identity**를 사용하여 사용자 인증 및 권한 관리를 구현합니다:

- **ApplicationUser**: `IdentityUser`를 확장하여 `DisplayName`, `Bio`, `ProfileImageUrl` 등의 추가 속성 포함
- **ApplicationRole**: `IdentityRole`을 확장하여 `Description`, `CreatedAt` 등의 추가 속성 포함
- **역할**: `Admin`, `User` 두 가지 역할이 데이터베이스 시드 시 자동 생성됨
- **관리자 계정**: 앱 시작 시 자동으로 생성됨 (admin@southmw.com / Admin123!)

인증 상태 확인:
```razor
<AuthorizeView>
    <Authorized>
        <p>환영합니다, @context.User.Identity?.Name!</p>
    </Authorized>
    <NotAuthorized>
        <p>로그인이 필요합니다.</p>
    </NotAuthorized>
</AuthorizeView>
```

권한 정책:
- **RequireAdminRole**: 관리자만 접근 가능
- **RequireUserRole**: 일반 사용자 또는 관리자 접근 가능

#### DbContextFactory 패턴

서비스는 Interactive 컴포넌트에서 발생하는 연결 풀링 문제를 피하기 위해 직접 DbContext 주입 대신 `IDbContextFactory<ApplicationDbContext>`를 사용합니다:

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

각 메서드는 새로운 컨텍스트 인스턴스를 생성하고 작업 완료 시 자동으로 dispose합니다. 이는 Interactive Blazor 시나리오에서 스레드 안전성을 위해 매우 중요합니다.

#### 서비스 레이어 아키텍처

모든 데이터베이스 작업은 서비스 인터페이스를 통해 이루어집니다:

```
컴포넌트 → 서비스 → DbContext → 데이터베이스
```

주요 서비스:
- **IBoardService / BoardService**: 게시글 CRUD, 조회수 증가, 페이징
- **ICommentService / CommentService**: 댓글 CRUD 작업
- **IFileUploadService / FileUploadService**: 파일 업로드/삭제
- **ICategoryService / CategoryService**: 카테고리 관리 (CRUD, 활성화/비활성화)
- **IUserService / UserService**: 사용자 관리 (프로필 업데이트, 통계)
- **IRoleService / RoleService**: 역할 관리
- **UserStateService**: 현재 로그인한 사용자 상태 관리 (Scoped 서비스)

서비스는 [Program.cs:82-88](BoardApp.WebApp/Program.cs#L82-L88)에서 **Scoped** 수명으로 등록됩니다.

#### 엔티티 관계 및 네비게이션 속성

```
ApplicationUser (1) → (N) Boards       [SetNull on delete]
ApplicationUser (1) → (N) Comments     [SetNull on delete]
ApplicationUser (1) → (N) Categories   [SetNull on delete]

Category (1) → (N) Boards              [Restrict on delete]

Board (1) → (N) Comments               [Cascade delete]
Board (1) → (N) BoardAttachments       [Cascade delete]
```

중요한 삭제 동작:
- **Board 삭제**: 관련된 모든 Comments와 Attachments가 자동으로 삭제됨 (Cascade)
- **User 삭제**: Board/Comment의 작성자 정보가 null로 설정됨 (SetNull), 게시글은 보존됨
- **Category 삭제**: Category를 사용하는 Board가 있으면 삭제 불가 (Restrict)

이러한 관계는 [ApplicationDbContext.cs:20-127](BoardApp.WebApp/Data/ApplicationDbContext.cs#L20-L127)의 `OnModelCreating`에서 구성됩니다.

### 인증 엔드포인트

애플리케이션은 폼 기반 인증을 위한 세 가지 POST 엔드포인트를 제공합니다:

- **POST /account/perform-login**: 이메일/비밀번호 로그인
- **POST /account/perform-register**: 새 사용자 등록
- **POST /account/perform-logout**: 로그아웃

이 엔드포인트들은 [Program.cs:185-252](BoardApp.WebApp/Program.cs#L185-L252)에서 정의됩니다.

Google OAuth 인증도 구성되어 있으나, `appsettings.json`에 ClientId와 ClientSecret을 설정해야 활성화됩니다.

### 파일 업로드 시스템

파일은 `wwwroot/uploads/boards/{boardId}/`에 저장되며, 충돌 방지를 위해 GUID가 접두사로 붙은 파일명을 사용합니다. `FileUploadService`는 다음을 처리합니다:

- 물리적 파일 저장
- 데이터베이스 레코드 생성 (BoardAttachment)
- 파일 삭제 (물리적 파일 및 DB 레코드 모두)
- 이미지/비디오 미리보기를 위한 MIME 타입 감지

**중요:** 리치 텍스트 에디터의 base64 이미지를 지원하기 위해 Kestrel과 SignalR이 100MB 메시지를 허용하도록 구성되어 있습니다([Program.cs:12-26](BoardApp.WebApp/Program.cs#L12-L26) 참조).

### 데이터베이스 구성

**연결 문자열:** `appsettings.json`에 위치
- 데이터베이스: `BoardWebApp`
- 서버: `127.0.0.1`의 SQL Server
- 인증: SQL Server 자격 증명 (sa/cloocus@2025)
- TrustServerCertificate: True (개발 환경)

**현재 스키마:**
- **Identity 테이블**: AspNetUsers, AspNetRoles, AspNetUserRoles 등 (ASP.NET Identity 표준 테이블)
- **Boards**: 게시글 (Title, Content, Author, AuthorId, CategoryId, ViewCount, CreatedAt, UpdatedAt)
- **Comments**: 댓글 (Content, Author, AuthorId, BoardId, CreatedAt, UpdatedAt)
- **BoardAttachments**: 첨부파일 (FileName, FilePath, ContentType, FileSize, BoardId, UploadedAt)
- **Categories**: 카테고리 (Name, Description, Color, DisplayOrder, IsPinned, IsActive, CreatedById, CreatedAt, UpdatedAt)

모든 타임스탬프는 일관성을 위해 SQL Server의 `GETDATE()`를 사용합니다.

**역할 시드 데이터:**
- Admin 역할: "관리자 역할"
- User 역할: "일반 사용자 역할"

**관리자 계정 자동 생성:**
앱 시작 시 [Program.cs:112-163](BoardApp.WebApp/Program.cs#L112-L163)에서 기본 관리자 계정이 자동으로 생성됩니다 (존재하지 않는 경우).

## UI 컴포넌트 및 MudBlazor

### 테마 커스터마이징

커스텀 테마는 [Components/Layout/MainLayout.razor](BoardApp.WebApp/Components/Layout/MainLayout.razor)에 정의되어 있습니다:

```csharp
private MudTheme _customTheme = new MudTheme()
{
    PaletteLight = new PaletteLight()
    {
        Primary = "#667eea",      // 보라색 그라데이션 주색상
        Secondary = "#764ba2",    // 진한 보라색
        Success = "#06d6a0",      // 민트 그린
        Info = "#4cc9f0",         // 하늘색
        Warning = "#ffd166",      // 노란색
        Error = "#ef476f",        // 핑크
    }
};
```

디자인 시스템은 그라데이션 배경을 광범위하게 사용합니다:
- 주 그라데이션: `linear-gradient(135deg, #667eea 0%, #764ba2 100%)`
- 일관된 border-radius: 8px, 12px, 16px
- 간격 시스템: 4px, 8px, 12px, 16px, 24px, 32px

### 리치 텍스트 에디터

콘텐츠 편집에 `BlazoredTextEditor`(Quill.js 래퍼)를 사용합니다. 콘텐츠는 **HTML 마크업**으로 데이터베이스에 저장됩니다.

에디터에서 콘텐츠 가져오기:
```csharp
if (quillEditor != null)
{
    board.Content = await quillEditor.GetHTML();
}
```

⚠️ **보안 주의**: 현재 HTML 콘텐츠가 sanitize되지 않습니다. 프로덕션 환경에서는 HtmlSanitizer 같은 라이브러리를 사용하여 XSS 공격을 방지하세요.

### 주요 MudBlazor 컴포넌트

- **MudStack**: Flexbox 레이아웃 컨테이너 (CSS flex 대체)
- **MudPaper**: elevation이 있는 카드/표면
- **MudTable**: 정렬, 페이징 기능이 있는 데이터 테이블
- **MudFileUpload**: 드래그 앤 드롭 파일 선택기
- **MudSnackbar**: 토스트 알림 (`ISnackbar` 주입을 통해)
- **MudDialog**: 모달 대화상자 (`IDialogService` 주입을 통해)

## 코드베이스 작업하기

### 새 기능 추가하기

1. **모델 레이어**: [Models/](BoardApp.WebApp/Models/)에 엔티티 추가/수정
2. **마이그레이션**: `dotnet ef migrations add`로 EF Core 마이그레이션 생성
3. **서비스 레이어**: [Services/](BoardApp.WebApp/Services/)에 인터페이스와 구현 추가
4. **서비스 등록**: [Program.cs](BoardApp.WebApp/Program.cs)의 DI에 추가
5. **UI 컴포넌트**: [Components/Pages/](BoardApp.WebApp/Components/Pages/)에 `.razor` 파일 생성
6. **라우트**: 컴포넌트 상단에 `@page` 지시문 추가
7. **권한 확인**: 필요한 경우 `@attribute [Authorize]` 또는 `@attribute [Authorize(Policy = "RequireAdminRole")]` 추가

### 데이터베이스 스키마 수정하기

엔티티 모델 변경 시:
1. [Models/](BoardApp.WebApp/Models/)의 모델 클래스 업데이트
2. 필요한 경우 [ApplicationDbContext.cs](BoardApp.WebApp/Data/ApplicationDbContext.cs)의 `OnModelCreating` 업데이트
3. 마이그레이션 생성: `cd BoardApp.WebApp && dotnet ef migrations add <설명이름>`
4. `Migrations/` 폴더에서 생성된 마이그레이션 검토
5. 앱 실행(자동 마이그레이션) 또는 `dotnet ef database update`로 수동 적용

**Identity 관련 변경:**
- ApplicationUser 또는 ApplicationRole 모델을 변경한 경우, Identity 테이블도 함께 마이그레이션됨
- Identity 컬럼을 직접 수정하지 말고, 커스텀 속성만 추가할 것

### 폼 작업하기

컴포넌트는 `DataAnnotationsValidator`와 함께 `EditForm`을 사용합니다:

```razor
<EditForm Model="@board" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <MudTextField @bind-Value="board.Title" For="@(() => board.Title)" />
    <MudButton ButtonType="ButtonType.Submit">저장</MudButton>
</EditForm>
```

모델 클래스의 검증 속성(예: `[Required]`, `[StringLength]`)이 자동으로 적용됩니다.

### 인증이 필요한 작업 구현하기

게시글 작성 시 현재 사용자 정보 가져오기:

```razor
@inject UserStateService UserState

@code {
    private async Task CreateBoard()
    {
        var currentUser = await UserState.GetCurrentUserAsync();
        if (currentUser == null)
        {
            // 로그인 페이지로 리다이렉트
            NavigationManager.NavigateTo("/account/login");
            return;
        }

        board.AuthorId = currentUser.Id;
        board.Author = currentUser.DisplayName ?? currentUser.Email ?? "Unknown";

        await BoardService.CreateBoardAsync(board);
    }
}
```

## 중요 사항

### 보안 고려사항

✅ **인증 시스템 구현됨**: ASP.NET Identity를 사용하여 사용자 인증 및 권한 관리가 구현되어 있습니다.

⚠️ **주의사항**:
- **HTML 콘텐츠 Sanitization**: 리치 에디터 콘텐츠가 sanitize되지 않습니다. 프로덕션에서는 HtmlSanitizer 라이브러리 사용 권장
- **파일 업로드 보안**: 현재 파일 타입 검증이 클라이언트 측에서만 이루어집니다. 서버 측 검증 추가 권장
- **Google OAuth 설정**: `appsettings.json`의 Google ClientId/ClientSecret은 환경 변수나 Azure Key Vault 같은 안전한 방식으로 관리할 것

**소유권 확인:**
- Board와 Comment는 각각 AuthorId를 통해 작성자와 연결됨
- 게시글/댓글 수정 및 삭제 시 소유권 확인 로직 구현 필요
- 관리자는 모든 게시글/댓글을 관리할 수 있어야 함

### 파일 업로드 제약사항

- 최대 파일 크기: 10 MB (클라이언트 측 검증만)
- 업로드당 최대 파일 수: 10개
- 허용 형식: `image/*,video/*,.pdf,.doc,.docx,.xls,.xlsx,.zip`
- 파일 저장 위치: `wwwroot/uploads/boards/{boardId}/`
- Kestrel 최대 요청 크기: 100 MB (base64 에디터 콘텐츠용)

### 한글 언어

이 애플리케이션은 UI 레이블과 메시지에 한글을 사용합니다:
- 게시판 = Board/Bulletin
- 게시글 = Post
- 댓글 = Comment
- 첨부파일 = Attachment
- 작성자 = Author
- 카테고리 = Category

## 일반적인 작업

### 새 서비스 추가하기

1. 인터페이스 생성: [Services/IMyService.cs](BoardApp.WebApp/Services/)
2. 구현체 생성: [Services/MyService.cs](BoardApp.WebApp/Services/)
3. [Program.cs](BoardApp.WebApp/Program.cs)에 등록:
   ```csharp
   builder.Services.AddScoped<IMyService, MyService>();
   ```
4. 컴포넌트에 주입:
   ```razor
   @inject IMyService MyService
   ```

### 새 인증 정책 추가하기

[Program.cs](BoardApp.WebApp/Program.cs)의 Authorization 섹션에 정책 추가:

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"))
    .AddPolicy("MyCustomPolicy", policy => policy.RequireClaim("CustomClaim"));
```

컴포넌트에서 사용:
```razor
@attribute [Authorize(Policy = "MyCustomPolicy")]
```

### 데이터베이스 초기화 및 재설정

```bash
# 완전히 새로 시작 (모든 데이터 삭제)
cd BoardApp.WebApp
dotnet ef database drop --force
dotnet run  # 최신 스키마로 자동 마이그레이션 및 시드 데이터 생성
```

앱 시작 시 자동으로 실행되는 작업:
1. 데이터베이스 마이그레이션 적용
2. Admin, User 역할 시드 (없는 경우)
3. 기본 관리자 계정 생성 (없는 경우)

### 카테고리 관리

카테고리는 게시글을 조직화하는 데 사용됩니다:

```csharp
@inject ICategoryService CategoryService

@code {
    private List<Category> categories = new();

    protected override async Task OnInitializedAsync()
    {
        categories = await CategoryService.GetActiveCategoriesAsync();
    }
}
```

카테고리 속성:
- **IsPinned**: 상단 고정 카테고리 (중요한 카테고리)
- **IsActive**: 활성화 상태 (비활성 카테고리는 숨김)
- **DisplayOrder**: 표시 순서 (오름차순)
- **Color**: 카테고리 색상 (Hex 코드, 예: #FF5733)

## 구성 파일

- [appsettings.json](BoardApp.WebApp/appsettings.json): 데이터베이스 연결 문자열, Google OAuth 설정, 로깅 구성
- [launchSettings.json](BoardApp.WebApp/Properties/launchSettings.json): 개발 서버 포트 및 프로필
- `*.csproj`: NuGet 패키지 참조 및 SDK 구성
- [Program.cs](BoardApp.WebApp/Program.cs): 서비스 등록, 인증/권한 설정, 미들웨어 파이프라인, 자동 마이그레이션

## 의존성

주요 NuGet 패키지:
- **MudBlazor 8.13.0**: Material Design UI 컴포넌트
- **Blazored.TextEditor 1.1.3**: 리치 텍스트 에디터 (Quill.js)
- **Microsoft.EntityFrameworkCore.SqlServer 9.0.10**: SQL Server 프로바이더
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.10**: ASP.NET Identity EF Core 통합
- **Microsoft.AspNetCore.Identity.UI 9.0.10**: Identity UI 컴포넌트
- **Microsoft.AspNetCore.Authentication.Google 9.0.10**: Google OAuth 인증
- **Microsoft.AspNetCore.Components.WebAssembly 9.0.10**: Blazor WASM 런타임

모든 패키지는 .NET 9.0을 대상으로 합니다.

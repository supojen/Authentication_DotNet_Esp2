# 目錄

* 背景知識
    - 有關於 Autentication Schema 的一些事
* Identity Authentication 補充知識
    - 一個加速產生 claimsprincipal 的方法 (UserClaimsFactory<>)
* .Net Core Identity Basic
    - .Net Core Identity 的基本配置跟使用

<br><br><br><br><br>
# 背景知識
<br>

### 有關於 Authentication Schema
___
* Authentication 可以使用 cookie or jwt 的方式, 如果選擇了其中之一, 又要在底下被置schema。 例如, 我選擇了 cookie 的方式, 我把我的 cookie 命名為 "MyCookie", 在 "MyCookie" 底下, 我得再配置 DefaultAuthenticateScheme, DefaultChallengeScheme, DefaultSignInScheme 等等
* Authentication Schema 是給 Authentication Middelware 在做身分認證時可以使用的方案, 可以有以下很多種 Schema, Authentication Middelware 會依據不同的情況使用不同方案

* 方案種類
    1. DefaultScheme
        - if specified, all the other defaults will fallback to this value

    1. DefaultAuthenticateScheme
        - if specified, AuthenticateAsync() will use this scheme, and also the AuthenticationMiddleware added by UseAuthentication() will use this scheme to set context.User automatically. (Corresponds to AutomaticAuthentication)

    1. DefaultChallengeScheme
        - if specified, ChallengeAsync() will use this scheme, [Authorize] with policies that don't specify schemes will also use this

    1. DefaultSignInScheme
        - It is used by SignInAsync() and also by all of the remote auth schemes like Google/Facebook/OIDC/OAuth, typically this would be set to a cookie.

    1. DefaultSignOutScheme
        - It is used by SignOutAsync() falls back to DefaultSignInScheme

    1. DefaultForbidScheme
        - It is used by ForbidAsync(), falls back to DefaultChallengeScheme


<br>

### 使用 .Net Core 時, 注意 Authentication Schema 的名字
___
* Related documentation
    - [IdentityConstants](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identityconstants?view=aspnetcore-2.2)
    - [IdentityConstants.ApplicationScheme](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.identityconstants.applicationscheme?view=aspnetcore-2.2) 

* .Net Core Authentication Schema 的配置
    ```c#
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    ```

<br><br><br><br><br>
# Identity Authentication 補充知識
<br>

### 一個幫助產生 ClaimsPrincipal 的方法
___
* 使用 UserClaimsPrincipalFactory<> (這種方法對於日後要自訂一claim 給 policy 使用很有幫助)
* 創建範例
    ```c#
    public class SelfDefineUserClaimsPrincipalFactory : 
        UserClaimsPrincipalFactory<IdentityUser>
    {
        public SelfDefineUserClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {}


        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var claims = await base.GenerateClaimsAsync(user);
            return claims;
        }
    }
    ```
* 使用範例
    ```c#
    var principal  = await _userClaimsPrincipalFactory.CreateAsync(user);
    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,principal);
    ```




<br><br><br><br><br>
# .Net Core Identity Basic
<br>

### 配置 DbContext 
___
* 配置一個 DbContext 專門在處存使用者資訊的
1. 安裝 Nuget Package
    > Microsoft.EntityFrameworkCore <br>
    > Microsoft.EntityFrameworkCore.InMemory 
1. 創建一個 DbContext 從繼承 IdentityDbContext 來的
    ```c#
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {}
    }
    ```
1. 登記剛剛創建出來的 DbContext
    ```c#
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase("Memory");
    });
    ```

<br>

### 對 .Net Core Identity 進行基本配置
___ 

1. 安裝 Nuget Package
    > Microsoft.AspNetCore.Identity.EntityFrameworkCore
1. Register .Net Core Identity
    - 這步會完成以下幾件事
    1. .Net Core 會自動產生 entity IdentityUser
    1. .Net Core 會自動 register 一些 service 像是 UsermManager 等等, 方便管理使用者用
    1. AddEntityFrameWorkStores 是為了讓自動 regsiter 的 service 可以從 database 內取出使用者的資料
    1. AddDefaultTokenProviders 會產生一個 Token Provider 供 Service 使用
    1. 會自動登記 Authentication (defualt 是使用 Cookie) & Authentication Schema
    - 範例
        ```c#
        services.AddIdentity<IdentityUser, IdentityRole>(options => {})
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        ```
1. 對 Cookie Authentication 進行一些變更
    ```c#
    services.ConfigureApplicationCookie(options => {
        options.Cookie.Name = "Grandpa.Cookie";
        options.LoginPath = "/Home/Login";
    });
    ```

<br>

### 在 Controller 裡使用
___
* 分三件事
1. Regsiter
    ```c#
    var user = await _userManager.FindByNameAsync(userName);
    if(user == null)
    {
        user = new IdentityUser 
        {
            UserName = userName,
        };
        var result = await _userManager.CreateAsync(user,password);
        if(result.Succeeded)
        {
            // Register 成功後要幹嘛
        }
    }
    ```
1. Login
    ```c#
    var user = await _userManager.FindByNameAsync(userName);

    if(user != null)
    {
        var principal  = await _userClaimsPrincipalFactory.CreateAsync(user);
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,principal);
        return RedirectToAction("Secret");
    }
    ```
1. Logout
    ```c#
    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
    ```

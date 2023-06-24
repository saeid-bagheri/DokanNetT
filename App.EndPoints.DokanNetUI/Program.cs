using App.Domain.AppService.Admins.Commands;
using App.Domain.AppService.Admins.Queries;
using App.Domain.Core.AppServices.Admins.Commands;
using App.Domain.Core.AppServices.Admins.Queries;
using App.Domain.Core.AppServices.Sellers.Commands;
using App.Domain.Core.DataAccess;
using App.Domain.Core.Entities;
using App.Domain.Core.Services.Admins.Commands;
using App.Domain.Core.Services.Admins.Queries;
using App.Domain.Core.Services.Application.Queries;
using App.Domain.Core.Services.Common.Commands;
using App.Domain.Core.Services.Sellers.Commands;
using App.Domain.Core.Services.Sellers.Queries;
using App.Domain.Service.Admins.Commands;
using App.Domain.Service.Admins.Queries;
using App.Domain.Service.Application.Queries;
using App.Domain.Service.Common.Commands;
using App.Domain.Service.Sellers.Commands;
using App.Domain.Service.Sellers.Queries;
using App.EndPoints.DokanNetUI.AutoMapper;
using App.Infrastructures.Data.Repositories;
using App.Infrastructures.Data.Repositories.AutoMapper;
using App.Infrastructures.Data.Repositories.Repositories;
using App.Infrastructures.Db.SqlServer.Ef.Database;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    //send user to a page that said your access is denied!
    options.AccessDeniedPath= "/";

});


var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Development.json", optional: false)
    .Build();

#region config dbContext
var connectionString = config.GetSection("ConnectionString").Value;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
#endregion  config dbContext
#region config identity
builder.Services.AddIdentity<AppUser,IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>();
#endregion config identity
#region config autoMapper
var configMapper = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMappingInfrastructures());
    cfg.AddProfile(new AutoMappingUI());
});
var mapper = configMapper.CreateMapper();
builder.Services.AddSingleton(mapper);
#endregion config autoMapper

#region dependency injection

//Common
builder.Services.AddScoped<IGetCities, GetCities>();
builder.Services.AddScoped<IGetCategories, GetCategories>();

//admins
builder.Services.AddScoped<ICloseStore, CloseStore>();
builder.Services.AddScoped<IConfirmComment, ConfirmComment>();
builder.Services.AddScoped<IConfirmProduct, ConfirmProduct>();
builder.Services.AddScoped<IDeleteProduct, DeleteProduct>();
builder.Services.AddScoped<IDeleteUser, DeleteUser>();
builder.Services.AddScoped<ILoginUser, LoginUser>();
builder.Services.AddScoped<IRegisterUser, RegisterUser>();
builder.Services.AddScoped<IUpdateProduct, UpdateProduct>();
builder.Services.AddScoped<IUpdateStore, UpdateStore>();
builder.Services.AddScoped<IUpdateUser, UpdateUser>();
builder.Services.AddScoped<IGetComments, GetComments>();
builder.Services.AddScoped<IGetProductById, GetProductById>();
builder.Services.AddScoped<IGetProducts, GetProducts>();
builder.Services.AddScoped<IGetStoreById, GetStoreById>();
builder.Services.AddScoped<IGetStores, GetStores>();
builder.Services.AddScoped<IGetUserById, GetUserById>();
builder.Services.AddScoped<IGetUserRolesByHttp, GetUserRolesByHttp>();
builder.Services.AddScoped<IGetUserRolesByUserName, GetUserRolesByUserName>();
builder.Services.AddScoped<IGetUsers, GetUsers>();
builder.Services.AddScoped<IGetInvoices, GetInvoices>();
builder.Services.AddScoped<ICreateSeller, CreateSeller>();
builder.Services.AddScoped<IAddRoleToUser, AddRoleToUser>();

//sellers
builder.Services.AddScoped<ICreateStore, CreateStore>();
builder.Services.AddScoped<IGetSellerById, GetSellerById>();
builder.Services.AddScoped<IUpdateSellerProfile, UpdateSellerProfile>();
builder.Services.AddScoped<IGetProductsByStoreId, GetProductsByStoreId>();
builder.Services.AddScoped<ICreateProduct, CreateProduct>();
builder.Services.AddScoped<IAddImageToProduct, AddImageToProduct>();
builder.Services.AddScoped<IAddCategory, AddCategory>();
builder.Services.AddScoped<IGetAuctionsByStoreId, GetAuctionsByStoreId>();
builder.Services.AddScoped<ICreateAuction, CreateAuction>();
builder.Services.AddScoped<IGetAuctionProductsByStoreId, GetAuctionProductsByStoreId>();
builder.Services.AddScoped<IReduceProductStock, ReduceProductStock>();
builder.Services.AddScoped<IIsExistProductInStoreByName, IsExistProductInStoreByName>();

//repositories
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IBuyerRepository, BuyerRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IMedalRepository, MedalRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISellerRepository, SellerRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

#endregion dependency injection

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapAreaControllerRoute(
    areaName: "Admin",
    name: "Areas",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    areaName: "Seller",
    name: "Areas",
    pattern: "Seller/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();

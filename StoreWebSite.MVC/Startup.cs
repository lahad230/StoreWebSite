using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreWebSite.DAL;
using StoreWebSite.DAL.Interfaces;
using StoreWebSite.MVC.Interfaces;
using StoreWebSite.MVC.Services;

namespace StoreWebSite.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        const string MyCorsPolicyName = "PolicyName";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {           
            services.AddDbContext<StoreDataContext>(options => options.UseLazyLoadingProxies().UseSqlite("Data Source = StoreDb.db"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IItemInCarts, ItemInCarts>();
            services.AddScoped<IProductsManagement, ProductManagement>();
            services.AddScoped<ICartIManagement, CartManagement>();
            services.AddScoped<IUserManagement, UserManagement>();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(MyCorsPolicyName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error/Error", "?statusCode={0}");
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Product}/{action=Index}/{id?}");
            });
        }
    }
}

using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using FaceImageAPI.Domain.AOP;
using FaceImageAPI.Repository.IRepository;
using FaceImageAPI.Repository.Repository;
using FaceImageAPI.Services.IService;
using FaceImageAPI.Services.Service;
using System.Reflection;

namespace FaceImageAPI.Ioc
{
    /// <summary>
    /// 控制台程序容器
    /// </summary>
    public static class Container
    {
        /// <summary>
        /// 容器
        /// </summary>
        public static Autofac.IContainer Instance;

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <returns></returns>
        public static void Init()
        {
            //新建容器构建器，用于注册组件和服务
            var builder = new ContainerBuilder();
            //自定义注册
            MyBuild(builder);
            //利用构建器创建容器
            Instance = builder.Build();
        }

        /// <summary>
        /// 自定义注册
        /// </summary>
        /// <param name="builder"></param>
        public static void MyBuild(ContainerBuilder builder)
        {
            //权限管理注入
            builder.RegisterType<AuthorityRepository>().As<IAuthorityRepository>();
            builder.RegisterType<AuthorityService>().As<IAuthorityService>();
            builder.RegisterType(typeof(ExceptionLogInterceptor));

            //员工管理注入
            builder.RegisterType<StaffManagementRepository>().As<IStaffManagementRepository>();
            builder.RegisterType<StaffManagementService>().As<IStaffManagementService>();


            ////Aop 拦截器注入
            //builder.RegisterType<ExceptionLogInterceptor>();//注册拦截器

        }
    }
}

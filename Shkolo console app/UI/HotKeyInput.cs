using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using RazorConsole.Core.Focus;
using RazorConsole.Core.Input;

// Lets the dynamically generated proxy below implement RazorConsole's internal interface.
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal sealed class IgnoresAccessChecksToAttribute : Attribute
    {
        public IgnoresAccessChecksToAttribute(string assemblyName) { }
    }
}

namespace Shkolo_console_app.UI
{
    // RazorConsole has no global shortcuts, and the interface that reads every key from the
    // console (IConsoleInput) is internal. HotKeyInput.Install puts a wrapper in front of it:
    // F1-F12 and (outside text boxes) 1-9 are turned into navigation events and removed from the
    // key stream; every other key goes on to the focused control as usual.
    public static class HotKeyInput
    {
        public delegate bool TryReadMouseDelegate(out TerminalMouseEvent mouseEvent);

        // Public so the generated proxy can call it. The proxy just forwards every call here.
        public sealed class Bridge
        {
            private readonly Func<bool> keyAvailable;
            private readonly Func<bool> decodes;
            private readonly Func<bool, ConsoleKeyInfo> readKey;
            private readonly TryReadMouseDelegate tryReadMouse;
            private readonly IServiceProvider services;
            private readonly NavigationState nav;

            public Bridge(object inner, Type iface, IServiceProvider services, NavigationState nav)
            {
                keyAvailable = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), inner, iface.GetProperty("KeyAvailable")!.GetMethod!);
                decodes = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), inner, iface.GetProperty("DecodesTerminalSequences")!.GetMethod!);
                readKey = (Func<bool, ConsoleKeyInfo>)Delegate.CreateDelegate(typeof(Func<bool, ConsoleKeyInfo>), inner, iface.GetMethod("ReadKey")!);
                tryReadMouse = (TryReadMouseDelegate)Delegate.CreateDelegate(typeof(TryReadMouseDelegate), inner, iface.GetMethod("TryReadMouse")!);
                this.services = services;
                this.nav = nav;
            }

            public bool KeyAvailable => keyAvailable();
            public bool DecodesTerminalSequences => decodes();
            public bool TryReadMouse(out TerminalMouseEvent mouseEvent) => tryReadMouse(out mouseEvent);

            public ConsoleKeyInfo ReadKey(bool intercept)
            {
                ConsoleKeyInfo key = readKey(intercept);

                // Swallowed keys: hand the app a key nothing reacts to instead.
                ConsoleKeyInfo nothing = new('\0', ConsoleKey.F24, false, false, false);

                if (IsHotKey(key, out string name))
                {
                    nav.Raise(name);
                    return nothing;
                }

                // A text box is focused but still locked (a tab was just opened with F-keys / 1-9):
                // Tab "enters" the box, and nothing is typed before that.
                if (nav.BoxFocused && !nav.TypingUnlocked)
                {
                    if (key.Key == ConsoleKey.Tab && (key.Modifiers & ConsoleModifiers.Shift) == 0)
                    {
                        nav.TypingUnlocked = true;
                        return nothing;
                    }

                    if (!char.IsControl(key.KeyChar) && key.KeyChar != '\0')
                        return nothing;
                }

                return key;
            }

            private bool IsHotKey(ConsoleKeyInfo key, out string name)
            {
                name = "";
                if ((key.Modifiers & (ConsoleModifiers.Control | ConsoleModifiers.Alt)) != 0) return false;

                // F1..F12 work everywhere.
                if (key.Key >= ConsoleKey.F1 && key.Key <= ConsoleKey.F12)
                {
                    name = "F" + (key.Key - ConsoleKey.F1 + 1);
                    return true;
                }

                // 1..9 work everywhere except while typing in a text box.
                if (key.KeyChar >= '1' && key.KeyChar <= '9')
                {
                    if (nav.Typing) return false;
                    name = key.KeyChar.ToString();
                    return true;
                }

                return false;
            }
        }

        public static void Install(IServiceCollection services)
        {
            Assembly razorConsole = typeof(RazorConsole.Components.TextButton).Assembly;
            Type iface = razorConsole.GetType("RazorConsole.Core.Input.IConsoleInput")!;

            services.AddSingleton<NavigationState>();

            ServiceDescriptor original = services.Last(d => d.ServiceType == iface);
            services.Remove(original);

            Type proxyType = BuildProxyType(iface, razorConsole.GetName().Name!);

            services.Add(new ServiceDescriptor(iface, sp =>
            {
                object inner = original.ImplementationFactory != null
                    ? original.ImplementationFactory(sp)
                    : original.ImplementationInstance ?? ActivatorUtilities.CreateInstance(sp, original.ImplementationType!);

                var bridge = new Bridge(inner, iface, sp, sp.GetRequiredService<NavigationState>());
                return Activator.CreateInstance(proxyType, bridge)!;
            }, original.Lifetime));
        }

        // class Proxy : IConsoleInput { every member just calls the same member on Bridge }
        private static Type BuildProxyType(Type iface, string razorConsoleAssemblyName)
        {
            var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("HotKeyProxyAssembly"), AssemblyBuilderAccess.Run);
            asm.SetCustomAttribute(new CustomAttributeBuilder(
                typeof(System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute)
                    .GetConstructor(new[] { typeof(string) })!,
                new object[] { razorConsoleAssemblyName }));

            ModuleBuilder module = asm.DefineDynamicModule("HotKeyProxyModule");
            TypeBuilder type = module.DefineType("HotKeyProxy", TypeAttributes.Public | TypeAttributes.Sealed, typeof(object), new[] { iface });
            FieldBuilder field = type.DefineField("bridge", typeof(Bridge), FieldAttributes.Private);

            ConstructorBuilder ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(Bridge) });
            ILGenerator il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);

            foreach (MethodInfo ifaceMethod in iface.GetMethods())
            {
                Type[] paramTypes = ifaceMethod.GetParameters().Select(p => p.ParameterType).ToArray();
                MethodInfo target = typeof(Bridge).GetMethod(ifaceMethod.Name, paramTypes)!;

                MethodBuilder method = type.DefineMethod(ifaceMethod.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                    ifaceMethod.ReturnType, paramTypes);
                il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field);
                for (int i = 1; i <= paramTypes.Length; i++) il.Emit(OpCodes.Ldarg, (short)i);
                il.Emit(OpCodes.Callvirt, target);
                il.Emit(OpCodes.Ret);
                type.DefineMethodOverride(method, ifaceMethod);
            }

            return type.CreateType();
        }
    }
}

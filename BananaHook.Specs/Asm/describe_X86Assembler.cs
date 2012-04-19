using System;
using BananaHook.Asm;
using NSpec;

namespace BananaHook.Specs.Asm
{
    class describe_X86Assembler : nspec
    {
        X86Assembler assembler;
        byte[] bytes;

        void before_each()
        {
            assembler = new X86Assembler();
        }

        void when_getting_the_bytes()
        {
            act = () => bytes = assembler.GetBytes();

            context["after pushing an address"] = () =>
            {
                before = () => assembler.Push(new IntPtr(0x1337));

                ItShouldEmitOpCode(OpCode.Push);
                ItShouldHaveTheBytes(1, 0x37, 0x13, 0, 0);
            };

            context["after returning"] = () =>
            {
                before = () => assembler.Retn();

                ItShouldEmitOpCode(OpCode.Retn);
            };

            context["after returning and purging the stack"] = () =>
            {
                before = () => assembler.Retn(0x10);

                ItShouldEmitOpCode(OpCode.RetnAndPurge);
                ItShouldHaveTheBytes(1, 0x10, 0);
            };
        }

        void ItShouldEmitOpCode(OpCode op)
        {
            it["should emit {0}".With(op)] = () => bytes[0].should_be((byte)op);
        }

        void ItShouldHaveTheBytes(int index, params byte[] b)
        {
            it["should have the correct bytes"] = () =>
            {
                for (int i = 0; i < b.Length; i++)
                {
                    bytes[index + i].should_be(b[i]);
                }
            };
        }
    }
}
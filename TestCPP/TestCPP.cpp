#include <iostream>
#include <vector>
#include <Windows.h>

typedef void(__thiscall* PrintChatFunc)(void* thisPtr, const char* msg);
DWORD64 funcPtr = 0xDEADBEEF1;
DWORD64 thisPtr = 0xDEADBEEF2;
DWORD64 msgPtr = 0xDEADBEEF3;
void PrintChat()
{
    const auto myPrintChat = reinterpret_cast<PrintChatFunc>(funcPtr);
    myPrintChat(reinterpret_cast<void*>(thisPtr), (const char*)msgPtr);
}

int GetSize(byte* func) {
    int size = 0;
    while (*func != 0xCC)
    {
        std::cout << std::hex << (int)*func << std::endl;
        size++;
        func++;
    }
    size--;

    return size;
}

byte* GetFunctionBytes(byte* func, int size) {
    auto result = new byte[size];

    for (auto i = 0; i < size; i++) {

    }

    return result;
}

struct BuffInstance
{
    int a = 10;
    float b = 20;
};

struct SpellbookParamCast
{
    int a = 33;
    float b = 44;
};

int main(int argc, char* argv[])
{
    auto s = std::make_shared<SpellbookParamCast>();
    std::vector<std::pair<BuffInstance*, std::shared_ptr<SpellbookParamCast>*>>* test = new  std::vector<std::pair<BuffInstance*, std::shared_ptr<SpellbookParamCast>*>>();
    test->push_back({new BuffInstance(), &s });
    test->push_back({ new BuffInstance(), &s });

    std::cout << std::hex << (DWORD64)test << std::endl;
    std::cout << std::hex << (DWORD64)test->begin()._Ptr << std::endl;
    std::cout << std::hex << (DWORD64)test->end()._Ptr << std::endl;
    
    //0x0 Start
    //0x8 End

    //void* printChatAddress = reinterpret_cast<void*>(&PrintChat);
    //std::cout << "0x" << std::hex << printChatAddress << " " << GetSize((byte*)printChatAddress) << std::endl;
    //PrintChat();
    while (true)
    {
        Sleep(1);
    }
    return 0;
}

function main()
{
    for i = 1 to 5
    {
        print(doit())
    }
    input()
}

function doit(): int
{
    return rnd(0, 50)
}

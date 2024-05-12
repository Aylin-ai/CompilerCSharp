function main()
{
    var a = 0
    for i = 1 to 5
    {
        a += i
        print("Example " + string(i))
        for j = 1 to 5
            print(rnd(i, i + rnd(j, j + 10)))
    }
    print(a)
}

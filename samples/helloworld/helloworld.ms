function main()
{
    let name = readLine("Say my name")
    print("Hello " + name)
}

function readLine(prompt: string): string
{
    print(prompt)
    let name = input()
    return name;
}
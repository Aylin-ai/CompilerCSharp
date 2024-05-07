function main()
{
    let name = readLine("Say my name")
    print("Hello " + name)
    input()
}

function readLine(prompt: string): string
{
    print(prompt)
    let name = input()
    return name;
}
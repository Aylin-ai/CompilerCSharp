function main(){
var name = getName()
print(name)
}

function getName(): string
{
    print("What is your name?")
    var name: string = input()
    return name;
}